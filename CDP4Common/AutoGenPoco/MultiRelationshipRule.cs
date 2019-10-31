// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiRelationshipRule.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Yevhen Ikonnykov
//
//    This file is part of CDP4-SDK Community Edition
//
//    The CDP4-SDK Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-SDK Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// <summary>
//   This is an auto-generated POCO Class. Any manual changes to this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.SiteDirectoryData
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
    /// representation of a validation rule for MultiRelationships that relate (potentially) more than two CategorizableThings
    /// </summary>
    [Container(typeof(ReferenceDataLibrary), "Rule")]
    public sealed partial class MultiRelationshipRule : Rule
    {
        /// <summary>
        /// Representation of the default value for the accessRight property of a PersonPermission for the affected class
        /// </summary>
        public new const PersonAccessRightKind DefaultPersonAccess = PersonAccessRightKind.SAME_AS_SUPERCLASS;

        /// <summary>
        /// Representation of the default value for the accessRight property of a PersonPermission for the affected class
        /// </summary>
        public new const ParticipantAccessRightKind DefaultParticipantAccess = ParticipantAccessRightKind.SAME_AS_SUPERCLASS;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiRelationshipRule"/> class.
        /// </summary>
        public MultiRelationshipRule()
        {
            this.RelatedCategory = new List<Category>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiRelationshipRule"/> class.
        /// </summary>
        /// <param name="iid">
        /// The unique identifier.
        /// </param>
        /// <param name="cache">
        /// The <see cref="ConcurrentDictionary{T, U}"/> where the current thing is stored.
        /// The <see cref="CacheKey"/> is the key used to store this thing.
        /// The key is a combination of this thing's identifier and the identifier of its <see cref="Iteration"/> container if applicable or null.
        /// </param>
        /// <param name="iDalUri">
        /// The <see cref="Uri"/> of this thing
        /// </param>
        public MultiRelationshipRule(Guid iid, ConcurrentDictionary<CacheKey, Lazy<CommonData.Thing>> cache, Uri iDalUri) : base(iid, cache, iDalUri)
        {
            this.RelatedCategory = new List<Category>();
        }

        /// <summary>
        /// Gets or sets the MaxRelated.
        /// </summary>
        /// <remarks>
        /// definition of the valid maximum number of <i>relatedThing</i> in a MultiRelationship that is a member of <i>relationshipCategory</i>
        /// Note 1: This can be used to define a cardinality constraint.
        /// Note 2: A value of -1 signifies that an unlimited number of <i>relatedThing</i> is valid.
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public int MaxRelated { get; set; }

        /// <summary>
        /// Gets or sets the MinRelated.
        /// </summary>
        /// <remarks>
        /// definition of the valid minimum number of <i>relatedThing</i> in a MultiRelationship that is a member of <i>relationshipCategory</i>
        /// Note: This can be used to define a cardinality constraint.
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public int MinRelated { get; set; }

        /// <summary>
        /// Gets or sets a list of Category.
        /// </summary>
        /// <remarks>
        /// reference to valid Categories for <i>relatedThing</i> of implied MultiRelationships
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public List<Category> RelatedCategory { get; set; }

        /// <summary>
        /// Gets or sets the RelationshipCategory.
        /// </summary>
        /// <remarks>
        /// reference to the Category whose member MultiRelationships shall comply with this MultiRelationshipRule
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public Category RelationshipCategory { get; set; }

        /// <summary>
        /// Creates and returns a copy of this <see cref="MultiRelationshipRule"/> for edit purpose.
        /// </summary>
        /// <param name="cloneContainedThings">A value that indicates whether the contained <see cref="Thing"/>s should be cloned or not.</param>
        /// <returns>
        /// A cloned instance of <see cref="MultiRelationshipRule"/>.
        /// </returns>
        protected override Thing GenericClone(bool cloneContainedThings)
        {
            var clone = (MultiRelationshipRule)this.MemberwiseClone();
            clone.Alias = cloneContainedThings ? new ContainerList<Alias>(clone) : new ContainerList<Alias>(this.Alias, clone);
            clone.Definition = cloneContainedThings ? new ContainerList<Definition>(clone) : new ContainerList<Definition>(this.Definition, clone);
            clone.ExcludedDomain = new List<DomainOfExpertise>(this.ExcludedDomain);
            clone.ExcludedPerson = new List<Person>(this.ExcludedPerson);
            clone.HyperLink = cloneContainedThings ? new ContainerList<HyperLink>(clone) : new ContainerList<HyperLink>(this.HyperLink, clone);
            clone.RelatedCategory = new List<Category>(this.RelatedCategory);

            if (cloneContainedThings)
            {
                clone.Alias.AddRange(this.Alias.Select(x => x.Clone(true)));
                clone.Definition.AddRange(this.Definition.Select(x => x.Clone(true)));
                clone.HyperLink.AddRange(this.HyperLink.Select(x => x.Clone(true)));
            }

            clone.Original = this;
            clone.ResetCacheId();
            return clone;
        }

        /// <summary>
        /// Creates and returns a copy of this <see cref="MultiRelationshipRule"/> for edit purpose.
        /// </summary>
        /// <param name="cloneContainedThings">A value that indicates whether the contained <see cref="Thing"/>s should be cloned or not.</param>
        /// <returns>
        /// A cloned instance of <see cref="MultiRelationshipRule"/>.
        /// </returns>
        public new MultiRelationshipRule Clone(bool cloneContainedThings)
        {
            this.ChangeKind = ChangeKind.Update;
            return (MultiRelationshipRule)this.GenericClone(cloneContainedThings);
        }

        /// <summary>
        /// Validates the cardinalities of the properties of this <see cref="MultiRelationshipRule"/>.
        /// </summary>
        /// <returns>
        /// A list of potential errors.
        /// </returns>
        protected override IEnumerable<string> ValidatePocoCardinality()
        {
            var errorList = new List<string>(base.ValidatePocoCardinality());

            var relatedCategoryCount = this.RelatedCategory.Count();
            if (relatedCategoryCount < 1)
            {
                errorList.Add("The number of elements in the property RelatedCategory is wrong. It should be at least 1.");
            }

            if (this.RelationshipCategory == null || this.RelationshipCategory.Iid == Guid.Empty)
            {
                errorList.Add("The property RelationshipCategory is null.");
                this.RelationshipCategory = SentinelThingProvider.GetSentinel<Category>();
                this.sentinelResetMap["RelationshipCategory"] = () => this.RelationshipCategory = null;
            }

            return errorList;
        }

        /// <summary>
        /// Resolve the properties of the current <see cref="MultiRelationshipRule"/> from its <see cref="DTO.Thing"/> counter-part
        /// </summary>
        /// <param name="dtoThing">The source <see cref="DTO.Thing"/></param>
        internal override void ResolveProperties(DTO.Thing dtoThing)
        {
            if (dtoThing == null)
            {
                throw new ArgumentNullException("dtoThing");
            }

            var dto = dtoThing as DTO.MultiRelationshipRule;
            if (dto == null)
            {
                throw new InvalidOperationException(string.Format("The DTO type {0} does not match the type of the current MultiRelationshipRule POCO.", dtoThing.GetType()));
            }

            this.Alias.ResolveList(dto.Alias, dto.IterationContainerId, this.Cache);
            this.Definition.ResolveList(dto.Definition, dto.IterationContainerId, this.Cache);
            this.ExcludedDomain.ResolveList(dto.ExcludedDomain, dto.IterationContainerId, this.Cache);
            this.ExcludedPerson.ResolveList(dto.ExcludedPerson, dto.IterationContainerId, this.Cache);
            this.HyperLink.ResolveList(dto.HyperLink, dto.IterationContainerId, this.Cache);
            this.IsDeprecated = dto.IsDeprecated;
            this.MaxRelated = dto.MaxRelated;
            this.MinRelated = dto.MinRelated;
            this.ModifiedOn = dto.ModifiedOn;
            this.Name = dto.Name;
            this.RelatedCategory.ResolveList(dto.RelatedCategory, dto.IterationContainerId, this.Cache);
            this.RelationshipCategory = this.Cache.Get<Category>(dto.RelationshipCategory, dto.IterationContainerId) ?? SentinelThingProvider.GetSentinel<Category>();
            this.RevisionNumber = dto.RevisionNumber;
            this.ShortName = dto.ShortName;

            this.ResolveExtraProperties();
        }

        /// <summary>
        /// Generates a <see cref="DTO.Thing"/> from the current <see cref="MultiRelationshipRule"/>
        /// </summary>
        public override DTO.Thing ToDto()
        {
            var dto = new DTO.MultiRelationshipRule(this.Iid, this.RevisionNumber);

            dto.Alias.AddRange(this.Alias.Select(x => x.Iid));
            dto.Definition.AddRange(this.Definition.Select(x => x.Iid));
            dto.ExcludedDomain.AddRange(this.ExcludedDomain.Select(x => x.Iid));
            dto.ExcludedPerson.AddRange(this.ExcludedPerson.Select(x => x.Iid));
            dto.HyperLink.AddRange(this.HyperLink.Select(x => x.Iid));
            dto.IsDeprecated = this.IsDeprecated;
            dto.MaxRelated = this.MaxRelated;
            dto.MinRelated = this.MinRelated;
            dto.ModifiedOn = this.ModifiedOn;
            dto.Name = this.Name;
            dto.RelatedCategory.AddRange(this.RelatedCategory.Select(x => x.Iid));
            dto.RelationshipCategory = this.RelationshipCategory != null ? this.RelationshipCategory.Iid : Guid.Empty;
            dto.RevisionNumber = this.RevisionNumber;
            dto.ShortName = this.ShortName;

            dto.IterationContainerId = this.CacheKey.Iteration;
            dto.RegisterSourceThing(this);
            this.BuildDtoPartialRoutes(dto);
            return dto;
        }
    }
}
