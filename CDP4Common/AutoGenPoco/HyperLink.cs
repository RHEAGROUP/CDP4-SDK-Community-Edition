// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HyperLink.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated POCO Class. Any manual changes to this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.CommonData
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using CDP4Common.CommonData;
    using CDP4Common.DiagramData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    /// <summary>
    /// representation of a hyperlink consisting of a URI and a descriptive text
    /// </summary>
    [Container(typeof(DefinedThing), "HyperLink")]
    public sealed partial class HyperLink : Thing, IAnnotation
    {
        /// <summary>
        /// Representation of the default value for the accessRight property of a PersonPermission for the affected class
        /// </summary>
        public new const PersonAccessRightKind DefaultPersonAccess = PersonAccessRightKind.SAME_AS_CONTAINER;

        /// <summary>
        /// Representation of the default value for the accessRight property of a PersonPermission for the affected class
        /// </summary>
        public new const ParticipantAccessRightKind DefaultParticipantAccess = ParticipantAccessRightKind.SAME_AS_CONTAINER;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLink"/> class.
        /// </summary>
        public HyperLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLink"/> class.
        /// </summary>
        /// <param name="iid">
        /// The unique identifier.
        /// </param>
        /// <param name="cache">
        /// The <see cref="ConcurrentDictionary{T, U}"/> where the current thing is stored.
        /// The <see cref="Tuple{T}"/> of <see cref="Guid"/> and <see cref="Nullable{Guid}"/> is the key used to store this thing.
        /// The key is a combination of this thing's identifier and the identifier of its <see cref="Iteration"/> container if applicable or null.
        /// </param>
        /// <param name="iDalUri">
        /// The <see cref="Uri"/> of this thing
        /// </param>
        public HyperLink(Guid iid, ConcurrentDictionary<Tuple<Guid, Guid?>, Lazy<Thing>> cache, Uri iDalUri) : base(iid, cache, iDalUri)
        {
        }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <remarks>
        /// textual content of the annotation expressed in the natural language as
        /// specified in <i>languageCode</i>
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the LanguageCode.
        /// </summary>
        /// <remarks>
        /// code that defines the natural language in which the annotation is written
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the Uri.
        /// </summary>
        /// <remarks>
        /// value of the actual Universal Resource Identifier (URI)
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public string Uri { get; set; }

        /// <summary>
        /// Creates and returns a copy of this <see cref="HyperLink"/> for edit purpose.
        /// </summary>
        /// <param name="cloneContainedThings">A value that indicates whether the contained <see cref="Thing"/>s should be cloned or not.</param>
        /// <returns>
        /// A cloned instance of <see cref="HyperLink"/>.
        /// </returns>
        protected override Thing GenericClone(bool cloneContainedThings)
        {
            var clone = (HyperLink)this.MemberwiseClone();
            clone.ExcludedDomain = new List<DomainOfExpertise>(this.ExcludedDomain);
            clone.ExcludedPerson = new List<Person>(this.ExcludedPerson);

            if (cloneContainedThings)
            {
            }

            clone.Original = this;
            clone.ResetCacheId();
            return clone;
        }

        /// <summary>
        /// Creates and returns a copy of this <see cref="HyperLink"/> for edit purpose.
        /// </summary>
        /// <param name="cloneContainedThings">A value that indicates whether the contained <see cref="Thing"/>s should be cloned or not.</param>
        /// <returns>
        /// A cloned instance of <see cref="HyperLink"/>.
        /// </returns>
        public new HyperLink Clone(bool cloneContainedThings)
        {
            this.ChangeKind = ChangeKind.Update;
            return (HyperLink)this.GenericClone(cloneContainedThings);
        }

        /// <summary>
        /// Validates the cardinalities of the properties of this <see cref="HyperLink"/>.
        /// </summary>
        /// <returns>
        /// A list of potential errors.
        /// </returns>
        protected override IEnumerable<string> ValidatePocoCardinality()
        {
            var errorList = new List<string>(base.ValidatePocoCardinality());

            if (string.IsNullOrWhiteSpace(this.Content))
            {
                errorList.Add("The property Content is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(this.LanguageCode))
            {
                errorList.Add("The property LanguageCode is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(this.Uri))
            {
                errorList.Add("The property Uri is null or empty.");
            }

            return errorList;
        }

        /// <summary>
        /// Resolve the properties of the current <see cref="HyperLink"/> from its <see cref="DTO.Thing"/> counter-part
        /// </summary>
        /// <param name="dtoThing">The source <see cref="DTO.Thing"/></param>
        internal override void ResolveProperties(DTO.Thing dtoThing)
        {
            if (dtoThing == null)
            {
                throw new ArgumentNullException("dtoThing");
            }

            var dto = dtoThing as DTO.HyperLink;
            if (dto == null)
            {
                throw new InvalidOperationException(string.Format("The DTO type {0} does not match the type of the current HyperLink POCO.", dtoThing.GetType()));
            }

            this.Content = dto.Content;
            this.ExcludedDomain.ResolveList(dto.ExcludedDomain, dto.IterationContainerId, this.Cache);
            this.ExcludedPerson.ResolveList(dto.ExcludedPerson, dto.IterationContainerId, this.Cache);
            this.LanguageCode = dto.LanguageCode;
            this.ModifiedOn = dto.ModifiedOn;
            this.RevisionNumber = dto.RevisionNumber;
            this.Uri = dto.Uri;

            this.ResolveExtraProperties();
        }

        /// <summary>
        /// Generates a <see cref="DTO.Thing"/> from the current <see cref="HyperLink"/>
        /// </summary>
        public override DTO.Thing ToDto()
        {
            var dto = new DTO.HyperLink(this.Iid, this.RevisionNumber);

            dto.Content = this.Content;
            dto.ExcludedDomain.AddRange(this.ExcludedDomain.Select(x => x.Iid));
            dto.ExcludedPerson.AddRange(this.ExcludedPerson.Select(x => x.Iid));
            dto.LanguageCode = this.LanguageCode;
            dto.ModifiedOn = this.ModifiedOn;
            dto.RevisionNumber = this.RevisionNumber;
            dto.Uri = this.Uri;

            dto.IterationContainerId = this.CacheId.Item2;
            dto.RegisterSourceThing(this);
            this.BuildDtoPartialRoutes(dto);
            return dto;
        }
    }
}