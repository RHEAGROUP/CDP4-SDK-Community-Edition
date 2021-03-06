﻿// -------------------------------------------------------------------------------------------------------------------------------
// <copyright file="CdpServicesDal.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexandervan Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of COMET-SDK Community Edition
//
//    The COMET-SDK Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET-SDK Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------------------

namespace CDP4ServicesDal
{
#if NETFRAMEWORK
    using System.ComponentModel.Composition;
#endif

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Dal;
    using CDP4Dal.Composition;
    using CDP4Dal.DAL;
    using CDP4Dal.DAL.ECSS1025AnnexC;
    using CDP4Dal.Exceptions;
    using CDP4Dal.Operations;
    
    using CDP4JsonSerializer;
    
    using NLog;

    using EngineeringModelSetup = CDP4Common.SiteDirectoryData.EngineeringModelSetup;
    using Thing = CDP4Common.DTO.Thing;
    using UriExtensions = CDP4Dal.UriExtensions;

    /// <summary>
    /// The purpose of the <see cref="CdpServicesDal"/> is to provide the Data Access Layer for CDP4 ECSS-E-TM-10-25
    /// Annex C, REST API
    /// </summary>
    [DalExport("COMET/CDP4 Services", "A COMET, or CDP4 Services Data Access Layer", "1.2.0", DalType.Web)]
#if NETFRAMEWORK
    [PartCreationPolicy(CreationPolicy.NonShared)]
#endif
    public class CdpServicesDal : Dal
    {
        /// <summary>
        /// The NLog Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="HttpClient"/> that is reused for each HTTP request by the current <see cref="Dal"/>.
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CdpServicesDal"/> class.
        /// </summary>
        public CdpServicesDal()
        {
            this.Serializer = new Cdp4JsonSerializer(this.MetaDataProvider, this.DalVersion);
        }

        /// <summary>
        /// Gets or sets the <see cref="Cdp4JsonSerializer"/>
        /// </summary>
        public Cdp4JsonSerializer Serializer { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this <see cref="IDal"/> is read only.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }
        
        /// <summary>
        /// Write all the <see cref="Operation"/>s from an <see cref="OperationContainer"/> asynchronously.
        /// </summary>
        /// <param name="operationContainer">
        /// The provided <see cref="OperationContainer"/> to write
        /// </param>
        /// <returns>
        /// A list of <see cref="Thing"/>s that has been created or updated since the last Read or Write operation.
        /// </returns>
        public override async Task<IEnumerable<Thing>> Write(OperationContainer operationContainer, IEnumerable<string> files = null)
        {
            if (this.Credentials == null || this.Credentials.Uri == null)
            {
                throw new InvalidOperationException("The CDP4 DAL is not open.");
            }

            if (operationContainer == null)
            {
                throw new ArgumentNullException(nameof(operationContainer), $"The {nameof(operationContainer)} may not be null");
            }

            if (operationContainer.Operations.Count() == 0)
            {
                Logger.Debug("The operationContainer is empty, no round trip to the datasource is made");
                return Enumerable.Empty<Thing>();
            }

            var watch = Stopwatch.StartNew();

            var result = new List<Thing>();

            if (files != null && files.Any())
            {
                this.OperationContainerFileVerification(operationContainer, files);
            }
            
            var attribute = new QueryAttributes
            {
                RevisionNumber = operationContainer.TopContainerRevisionNumber
            };

            var postToken = operationContainer.Token;
            var resourcePath = $"{operationContainer.Context}{attribute}";

            var uriBuilder = this.GetUriBuilder(this.Credentials.Uri, ref resourcePath);

            Logger.Debug("Resource Path {0}: {1}", postToken, resourcePath);
            Logger.Debug("CDP4 Services POST: {0} - {1}", postToken, uriBuilder);

            var requestContent = this.CreateHttpContent(postToken, operationContainer, files);

            var requestsw = Stopwatch.StartNew();

            using (var httpResponseMessage = await this.httpClient.PostAsync(resourcePath, requestContent))
            {
                Logger.Info("CDP4 Services responded in {0} [ms] to POST {1}", requestsw.ElapsedMilliseconds, postToken);
                requestsw.Stop();

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    var errorResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                    var msg = $"The CDP4 Services replied with code {httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}: {errorResponse}";
                    Logger.Error(msg);
                    throw new DalWriteException(msg);
                }

                this.ProcessHeaders(httpResponseMessage);

                using (var resultStream = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    result.AddRange(this.Serializer.Deserialize(resultStream));

                    Guid iterationId;
                    if (this.TryExtractIterationIdfromUri(httpResponseMessage.RequestMessage.RequestUri, out iterationId))
                    {
                        this.SetIterationContainer(result, iterationId);
                    }
                }
            }

            watch.Stop();
            Logger.Info("Write Operation completed in {0} [ms]", watch.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Write all the <see cref="Operation"/>s from all the <see cref="OperationContainer"/>s.
        /// </summary>
        /// <param name="operationContainer">
        /// The provided list of <see cref="OperationContainer"/>s to write
        /// </param>
        /// <param name="files">
        /// The path to the files that need to be uploaded. If <paramref name="files"/> is null, then no files are to be uploaded
        /// </param>
        /// <returns>
        /// A list of <see cref="Thing"/>s that has been created or updated since the last Read or Write operation.
        /// </returns>
        public override Task<IEnumerable<Thing>> Write(IEnumerable<OperationContainer> operationContainer, IEnumerable<string> files = null)
        {
            throw new NotSupportedException("Writing multiple OperationContainers to the data-source is not supported");
        }

        /// <summary>
        /// Reads the data related to the provided <see cref="CDP4Common.EngineeringModelData.Iteration"/> from the data-source
        /// </summary>
        /// <param name="iteration">
        /// An instance of <see cref="CDP4Common.EngineeringModelData.Iteration"/> that needs to be read from the data-source
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <param name="attributes">
        /// An instance of <see cref="IQueryAttributes"/> to be used with the request
        /// </param>
        /// <returns>
        /// A list of <see cref="Thing"/> that are contained by the provided <see cref="CDP4Common.EngineeringModelData.EngineeringModel"/> including the Reference-Data.
        /// All the <see cref="Thing"/>s that have been updated since the last read will be returned.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <see cref="Session"/> property has not been set
        /// </exception>
        public override async Task<IEnumerable<Thing>> Read(CDP4Common.DTO.Iteration iteration, CancellationToken cancellationToken, IQueryAttributes attributes = null)
        {
            if (this.Session == null)
            {
                throw new InvalidOperationException("The Session may not be null and must be set prior to reading an Iteration");
            }

            // Get the RequiredRdl to load
            var siteDirectory = this.Session.Assembler.RetrieveSiteDirectory();
            var iterationSetup = siteDirectory.Model.SelectMany(mod => mod.IterationSetup).SingleOrDefault(it => it.IterationIid == iteration.Iid);
            if (iterationSetup == null)
            {
                throw new InvalidOperationException("The Iteration to open does not have any associated IterationSetup.");
            }

            var modelSetup = (EngineeringModelSetup)iterationSetup.Container;
            var modelReferenceDataLibrary = modelSetup.RequiredRdl.SingleOrDefault();

            if (modelReferenceDataLibrary == null)
            {
                throw new InvalidOperationException("The model to open does not have a Required Reference-Data-Library.");
            }

            var modelReferenceDataLibraryDto = modelReferenceDataLibrary.ToDto();

            var result = new List<Thing>();
            var referenceData = await this.Read(modelReferenceDataLibraryDto, cancellationToken);
            result.AddRange(referenceData);
            var engineeringModelData = await this.Read((Thing)iteration, cancellationToken);
            result.AddRange(engineeringModelData);
            return result;
        }

        /// <summary>
        /// Reads a physical file from a DataStore
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> that has a </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>an await-able <see cref="Task"/> that returns a <see cref="byte"/> array.</returns>
        public override async Task<byte[]> ReadFile(Thing thing, CancellationToken cancellationToken) 
        { 
            if (this.Credentials == null || this.Credentials.Uri == null)
            {
                throw new InvalidOperationException("The CDP4 DAL is not open.");
            }

            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing), $"The {nameof(thing)} may not be null");
            }

            var watch = Stopwatch.StartNew();

            var thingRoute = this.CleanUriFinalSlash(thing.Route);

            var resourcePath = $"{thingRoute}?includeFileData=true";

            var readToken = CDP4Common.Helpers.TokenGenerator.GenerateRandomToken();
            var uriBuilder = this.GetUriBuilder(this.Credentials.Uri, ref resourcePath);

            Logger.Debug("Resource Path {0}: {1}", readToken, resourcePath);
            Logger.Debug("CDP4Services GET {0}: {1}", readToken, uriBuilder);

            var requestsw = Stopwatch.StartNew();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, resourcePath);
            requestMessage.Headers.Add(Headers.CDPToken, readToken);

            using (var httpResponseMessage = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                Logger.Info("CDP4 Services responded in {0} [ms] to GET {1}", requestsw.ElapsedMilliseconds, readToken);
                requestsw.Stop();

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    var msg = $"The data-source replied with code {httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}";
                    Logger.Error(msg);
                    throw new DalReadException(msg);
                }

                this.ProcessHeaders(httpResponseMessage, true);

                cancellationToken.ThrowIfCancellationRequested();

                var multipartContent = await httpResponseMessage.Content.ReadAsMultipartAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var returned = await multipartContent.Contents[1].ReadAsByteArrayAsync();

                cancellationToken.ThrowIfCancellationRequested();

                watch.Stop();
                Logger.Info("JSON Deserializer completed in {0} [ms]", watch.ElapsedMilliseconds);

                return returned;
            }
        }

        /// <summary>
        /// Reads the data related to the provided <see cref="Thing"/> from the data-source
        /// </summary>
        /// <typeparam name="T">
        /// an type of <see cref="Thing"/>
        /// </typeparam>
        /// <param name="thing">
        /// An instance of <see cref="Thing"/> that needs to be read from the data-source
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <param name="attributes">
        /// An instance of <see cref="IQueryAttributes"/> to be passed along with the request
        /// </param>
        /// <returns>
        /// a list of <see cref="Thing"/> that are contained by the provided <see cref="Thing"/> including the <see cref="Thing"/> itself
        /// </returns>
        public override async Task<IEnumerable<Thing>> Read<T>(T thing, CancellationToken cancellationToken, IQueryAttributes attributes = null)
        {
            if (this.Credentials == null || this.Credentials.Uri == null)
            {
                throw new InvalidOperationException("The CDP4 DAL is not open.");
            }

            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing), $"The {nameof(thing)} may not be null");
            }

            var watch = Stopwatch.StartNew();

            if (attributes == null)
            {
                var includeReferenData = thing is ReferenceDataLibrary;

                attributes = this.GetIUriQueryAttribute(includeReferenData);
            }

            var thingRoute = this.CleanUriFinalSlash(thing.Route);

            var resourcePath = $"{thingRoute}{attributes?.ToString()}";

            var readToken = CDP4Common.Helpers.TokenGenerator.GenerateRandomToken();
            var uriBuilder = this.GetUriBuilder(this.Credentials.Uri, ref resourcePath);

            Logger.Debug("Resource Path {0}: {1}", readToken, resourcePath);
            Logger.Debug("CDP4Services GET {0}: {1}", readToken, uriBuilder);

            var requestsw = Stopwatch.StartNew();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, resourcePath);
            requestMessage.Headers.Add(Headers.CDPToken, readToken);

            using (var httpResponseMessage = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                Logger.Info("CDP4 Services responded in {0} [ms] to GET {1}", requestsw.ElapsedMilliseconds, readToken);
                requestsw.Stop();

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    var msg = $"The data-source replied with code {httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}";
                    Logger.Error(msg);
                    throw new DalReadException(msg);
                }

                this.ProcessHeaders(httpResponseMessage);

                using (var resultStream = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    var returned = this.Serializer.Deserialize(resultStream);

                    if (this.TryExtractIterationIdfromUri(httpResponseMessage.RequestMessage.RequestUri, out var iterationId))
                    {
                        this.SetIterationContainer(returned, iterationId);
                    }

                    watch.Stop();
                    Logger.Info("JSON Deserializer completed in {0} [ms]", watch.ElapsedMilliseconds);

                    return returned;
                }
            }
        }

        /// <summary>
        /// Creates the specified <see cref="Thing"/> on a data source
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is to be created
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="Thing"/>
        /// </typeparam>
        /// <returns>
        /// A list of <see cref="Thing"/> that have been updated since the last Read has been performed. This includes the thing that was created.
        /// </returns>
        public override IEnumerable<Thing> Create<T>(T thing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs an update to the <see cref="Thing"/> on the data-source
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is to be updated
        /// </param>
        /// <typeparam name="T">
        /// a type of <see cref="Thing"/>
        /// </typeparam>
        /// <returns>
        /// A list of <see cref="Thing"/> that have been updated since the last Read has been performed.
        /// </returns>
        public override IEnumerable<Thing> Update<T>(T thing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified <see cref="Thing"/> from the data-source
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is to be deleted
        /// </param>
        /// <typeparam name="T">
        /// a type of <see cref="Thing"/>
        /// </typeparam>
        /// <returns>
        /// A list of <see cref="Thing"/> that have been updated since the last Read has been performed.
        /// </returns>
        public override IEnumerable<Thing> Delete<T>(T thing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a connection to a data source <see cref="Uri"/> speci1fied by the provided <see cref="Credentials"/>
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Dal.Credentials"/> that are used to connect to the data source such as username, password and <see cref="Uri"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> that the services return when connecting to the <see cref="CDP4Common.SiteDirectoryData.SiteDirectory"/>.
        /// </returns>
        public override async Task<IEnumerable<Thing>> Open(Credentials credentials, CancellationToken cancellationToken)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials), $"The {nameof(credentials)} may not be null");
            }

            if (credentials.Uri == null)
            {
                throw new ArgumentNullException(nameof(credentials.Uri), $"The Credentials URI may not be null");
            }

            UriExtensions.AssertUriIsHttpOrHttpsSchema(credentials.Uri);

            var queryAttributes = new QueryAttributes
            {
                Extent = ExtentQueryAttribute.deep,
                IncludeReferenceData = false
            };
            
            var resourcePath = $"SiteDirectory{queryAttributes}";

            var openToken = CDP4Common.Helpers.TokenGenerator.GenerateRandomToken();

            this.httpClient = this.CreateHttpClient(credentials);

            var watch = Stopwatch.StartNew();
            
            var uriBuilder = this.GetUriBuilder(credentials.Uri, ref resourcePath);

            Logger.Debug("Resource Path {0}: {1}", openToken, resourcePath);
            Logger.Debug("CDP4Services Open {0}: {1}", openToken, uriBuilder);

            var requestsw = Stopwatch.StartNew();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, resourcePath);
            requestMessage.Headers.Add(Headers.CDPToken, openToken);

            using (var httpResponseMessage = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken: cancellationToken))            
            {
                Logger.Info("CDP4 Services responded in {0} [ms] to Open {1}", requestsw.ElapsedMilliseconds, openToken);
                requestsw.Stop();

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    var msg = $"The data-source replied with code {httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}";
                    Logger.Error(msg);
                    throw new DalReadException(msg);
                }

                watch.Stop();
                Logger.Info("CDP4Services Open {0}: {1} completed in {2} [ms]", openToken, uriBuilder, watch.ElapsedMilliseconds);

                this.ProcessHeaders(httpResponseMessage);
                    
                watch = Stopwatch.StartNew();

                using (var resultStream = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    var returned = this.Serializer.Deserialize(resultStream);

                    watch.Stop();
                    Logger.Info("JSON Deserializer completed in {0} [ms]", watch.ElapsedMilliseconds);

                    var returnedPerson = returned.OfType<CDP4Common.DTO.Person>().SingleOrDefault(x => x.ShortName == credentials.UserName);
                    if (returnedPerson == null)
                    {
                        throw new InvalidOperationException("User not found.");
                    }

                    this.Credentials = credentials;

                    return returned;
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="HttpClient"/>
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> used to set the connection and authentication settings as well as the proxy server settings
        /// </param>
        /// <returns>
        /// An instance of <see cref="HttpClient"/> with the DefaultRequestHeaders set
        /// </returns>
        private HttpClient CreateHttpClient(Credentials credentials)
        {
            HttpClient result;

            if (credentials.ProxySettings == null)
            {
                Logger.Debug("creating HttpClient without proxy");

                result = new HttpClient();
            }
            else
            {
                Logger.Debug("creating HttpClient with proxy: {0}", credentials.ProxySettings.Address);

                var proxy = new WebProxy(credentials.ProxySettings.Address);
                
                if (!string.IsNullOrEmpty(credentials.ProxySettings.UserName))
                {
                    var proxyCredential = new NetworkCredential(credentials.ProxySettings.UserName, credentials.ProxySettings.Password);
                    proxy.Credentials = proxyCredential;
                }

                var httpClientHandler = new HttpClientHandler()
                {
                    Proxy = proxy,
                    UseProxy = true
                };
                
                result = new HttpClient(httpClientHandler);
            }

            result.BaseAddress = credentials.Uri;
            result.DefaultRequestHeaders.Accept.Clear();
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", credentials.UserName, credentials.Password))));
            result.DefaultRequestHeaders.Add(Headers.AcceptCdpVersion, Headers.AcceptCdpVersionValue);
            result.DefaultRequestHeaders.Add("User-Agent", "CDP4 (ECSS-E-TM-10-25 Annex C.2) CDPServicesDal");

            return result;
        }

        /// <summary>
        /// Creates <see cref="HttpContent"/> that is added to a POST request
        /// </summary>
        /// <param name="token">
        /// The POST token that is used to track the POST request in a logger
        /// </param>
        /// <param name="operationContainer">
        /// The <see cref="OperationContainer"/> that contains the <see cref="Operation"/>s that are part of 
        /// the transaction that is sent to the DAL
        /// </param>
        /// <param name="files">
        /// A list of file-paths that is used to construct the <see cref="MultipartFormDataContent"/>
        /// </param>
        /// <returns>
        /// An instance of <see cref="StringContent"/> or <see cref="MultipartFormDataContent"/>
        /// </returns>
        private HttpContent CreateHttpContent(string token, OperationContainer operationContainer, IEnumerable<string> files = null)
        {
            var stream = new MemoryStream();
            this.ConstructPostRequestBodyStream(token, operationContainer, stream);
            var jsonContent = new StreamContent(stream);
            jsonContent.Headers.Add(Headers.ContentType, "application/json");
            jsonContent.Headers.Add(Headers.CDPToken, token);

            if (files == null)
            {
                return jsonContent;
            }
            else
            {
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(jsonContent);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);

                    using (var filestream = System.IO.File.OpenRead(file))
                    {
                        var contentStream = new MemoryStream();
                        filestream.CopyTo(contentStream);
                        contentStream.Position = 0;

                        var fileContent = new StreamContent(contentStream);
                        fileContent.Headers.Add("Content-Type", "application/octet-stream");
                        fileContent.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                        multipartContent.Add(fileContent);
                    }
                }

                return multipartContent;
            }
        }

        /// <summary>
        /// Constructs the JSON stream containing the full POST body of the given <see cref="OperationContainer"/>
        /// </summary>
        /// <param name="token">
        /// The POST token that is used to track the POST request in a logger
        /// </param>
        /// <param name="operationContainer">
        /// The <see cref="OperationContainer"/> that is serialized to a JSON stream
        /// </param>
        /// <param name="outputStream">
        /// The stream to which is written
        /// </param>
        internal void ConstructPostRequestBodyStream(string token, OperationContainer operationContainer, Stream outputStream)
        {
            var postOperation = new CdpPostOperation(this.MetaDataProvider, this.Session);

            // add the simple operations to the WSP container
            foreach (var operation in operationContainer.Operations)
            {
                postOperation.ConstructFromOperation(operation);
            }

            this.Serializer.SerializeToStream(postOperation, outputStream);
            outputStream.Position = 0;

            if (Logger.IsTraceEnabled)
            {
                using (var memoryStream = new MemoryStream())
                {
                    outputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        var postBody = streamReader.ReadToEnd();
                        Logger.Trace("POST JSON BODY {0} /r/n {1}", token, postBody);
                    }
                }

                outputStream.Position = 0;
            }
        }

        /// <summary>
        /// process the response to verify that the required headers are available
        /// </summary>
        /// <param name="httpResponseMessage">
        /// The <see cref="HttpResponseMessage"/> that is to be verified
        /// </param>
        /// <param name="allowMultiPart">Optional <see cref="bool"/> indicating if multipart/mixed content is allowed in the contentheader</param>
        private void ProcessHeaders(HttpResponseMessage httpResponseMessage, bool allowMultiPart = false)
        {
            var responseHeaders = httpResponseMessage.Headers;
            
            var cdpServerHeader = responseHeaders.SingleOrDefault(h => h.Key.ToLower(CultureInfo.InvariantCulture) == Headers.CDPServer.ToLower(CultureInfo.InvariantCulture));

            if (cdpServerHeader.Value == null)
            {
                throw new HeaderException($"Header {Headers.CDPServer} not found");
            }

            var cdpCommonHeader = responseHeaders.SingleOrDefault(h => h.Key.ToLower(CultureInfo.InvariantCulture) == Headers.CDPCommon.ToLower(CultureInfo.InvariantCulture));

            if (cdpCommonHeader.Value == null)
            {
                throw new HeaderException($"Header {Headers.CDPCommon} not found");
            }

            var contentHeaders = httpResponseMessage.Content.Headers;

            var mediaTypeHeader = contentHeaders.SingleOrDefault(h => h.Key.ToLower(CultureInfo.InvariantCulture) == Headers.ContentType.ToLower(CultureInfo.InvariantCulture));

            if (mediaTypeHeader.Value == null)
            {
                throw new HeaderException($"Header {Headers.ContentType} not found");
            }

            var headerString = Convert.ToString(mediaTypeHeader.Value.FirstOrDefault()).ToLower(CultureInfo.InvariantCulture);

            if (!this.IsCDP4ContentType(headerString, allowMultiPart))
            {
                throw new HeaderException($"Header Media-Type has incompatible value: {mediaTypeHeader.Value} ");
            }
        }

        /// <summary>
        /// Checks message content header for CDP4 tags an compatiple content-types
        /// </summary>
        /// <param name="headerString">The header <see cref="string"/></param>
        /// <param name="allowMultiPart">Indication if multipart Content-Type is allowed</param>
        /// <returns>true if a CDP4 content type is found, otherwise false</returns>
        private bool IsCDP4ContentType(string headerString, bool allowMultiPart)
        {
            var headerArray = headerString
                .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();

            if (!headerArray.Contains("ecss-e-tm-10-25") || !headerArray.Contains("version=1.0.0"))
            {
                return false;
            }

            if (!headerArray.Contains("application/json"))
            {
                if (!allowMultiPart || !headerArray.Contains("multipart/mixed"))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Close the <see cref="CdpServicesDal"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If the Data Access Layer is already closed 
        /// </exception>
        /// <remarks>
        /// Disposes of the underlying <see cref="HttpClient"/>
        /// </remarks>
        public override void Close()
        {
            if (this.Credentials == null)
            {
                throw new InvalidOperationException("An already closed Data Access Layer may not be closed again.");
            }

            if (this.httpClient != null)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
            }
            
            this.CloseSession();
        }

        /// <summary>
        /// Assertion that the provided string is a valid <see cref="Uri"/> to connect to
        /// a data-source with the current implementation of the <see cref="IDal"/>.
        /// </summary>
        /// <param name="uri">
        /// a string representing a <see cref="Uri"/>.
        /// </param>
        /// <returns>
        /// true when valid, false when invalid.
        /// </returns>
        /// <remarks>
        /// Only HTTP and HTTPS are valid.
        /// </remarks>
        public override bool IsValidUri(string uri)
        {
            try
            {
                var validUriAssertion = new Uri(uri);
                UriExtensions.AssertUriIsHttpOrHttpsSchema(validUriAssertion);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// gets the <see cref="QueryAttributes"/> associated to the <see cref="CdpServicesDal"/>
        /// </summary>
        /// <param name="includeReferenceData">
        /// Assertion to include reference data or not
        /// </param>
        /// <returns>
        /// the <see cref="QueryAttributes"/>
        /// </returns>
        private IQueryAttributes GetIUriQueryAttribute(bool includeReferenceData = false)
        {
            return includeReferenceData
                ? new QueryAttributes
                {
                    Extent = ExtentQueryAttribute.deep,
                    IncludeAllContainers = true,
                    IncludeReferenceData = true
                }
                : new QueryAttributes
                {
                    Extent = ExtentQueryAttribute.deep,
                    IncludeAllContainers = true
                };
        }
    }
}
