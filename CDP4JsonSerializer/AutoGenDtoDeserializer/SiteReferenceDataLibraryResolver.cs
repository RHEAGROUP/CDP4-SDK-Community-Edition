// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibraryResolver.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated Resolver class. Any manual changes on this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4JsonSerializer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The purpose of the <see cref="SiteReferenceDataLibraryResolver"/> is to deserialize a JSON object to a <see cref="SiteReferenceDataLibrary"/>
    /// </summary>
    public static class SiteReferenceDataLibraryResolver
    {
        /// <summary>
        /// Instantiate and deserialize the properties of a <paramref name="SiteReferenceDataLibrary"/>
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/> containing the data</param>
        /// <returns>The <see cref="SiteReferenceDataLibrary"/> to instantiate</returns>
        public static CDP4Common.DTO.SiteReferenceDataLibrary FromJsonObject(JObject jObject)
        {
            var iid = jObject["iid"].ToObject<Guid>();
            var revisionNumber = jObject["revisionNumber"].IsNullOrEmpty() ? 0 : jObject["revisionNumber"].ToObject<int>();
            var siteReferenceDataLibrary = new CDP4Common.DTO.SiteReferenceDataLibrary(iid, revisionNumber);

            if (!jObject["alias"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Alias.AddRange(jObject["alias"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["baseQuantityKind"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.BaseQuantityKind.AddRange(jObject["baseQuantityKind"].ToOrderedItemCollection());
            }

            if (!jObject["baseUnit"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.BaseUnit.AddRange(jObject["baseUnit"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["constant"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Constant.AddRange(jObject["constant"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["definedCategory"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.DefinedCategory.AddRange(jObject["definedCategory"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["definition"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Definition.AddRange(jObject["definition"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["excludedDomain"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ExcludedDomain.AddRange(jObject["excludedDomain"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["excludedPerson"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ExcludedPerson.AddRange(jObject["excludedPerson"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["fileType"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.FileType.AddRange(jObject["fileType"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["glossary"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Glossary.AddRange(jObject["glossary"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["hyperLink"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.HyperLink.AddRange(jObject["hyperLink"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["isDeprecated"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.IsDeprecated = jObject["isDeprecated"].ToObject<bool>();
            }

            if (!jObject["modifiedOn"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ModifiedOn = jObject["modifiedOn"].ToObject<DateTime>();
            }

            if (!jObject["name"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Name = jObject["name"].ToObject<string>();
            }

            if (!jObject["parameterType"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ParameterType.AddRange(jObject["parameterType"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["referenceSource"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ReferenceSource.AddRange(jObject["referenceSource"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["requiredRdl"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.RequiredRdl = jObject["requiredRdl"].ToObject<Guid?>();
            }

            if (!jObject["rule"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Rule.AddRange(jObject["rule"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["scale"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Scale.AddRange(jObject["scale"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["shortName"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.ShortName = jObject["shortName"].ToObject<string>();
            }

            if (!jObject["unit"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.Unit.AddRange(jObject["unit"].ToObject<IEnumerable<Guid>>());
            }

            if (!jObject["unitPrefix"].IsNullOrEmpty())
            {
                siteReferenceDataLibrary.UnitPrefix.AddRange(jObject["unitPrefix"].ToObject<IEnumerable<Guid>>());
            }

            return siteReferenceDataLibrary;
        }
    }
}