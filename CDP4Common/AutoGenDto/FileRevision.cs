// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRevision.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2018 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou
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
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.DTO
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    /// <summary>
    /// A Data Transfer Object representation of the <see cref="FileRevision"/> class.
    /// </summary>
    [DataContract]
    [Container(typeof(File), "FileRevision")]
    public sealed partial class FileRevision : Thing, INamedThing, ITimeStampedThing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRevision"/> class.
        /// </summary>
        public FileRevision()
        {
            this.FileType = new List<OrderedItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRevision"/> class.
        /// </summary>
        /// <param name="iid">
        /// The unique identifier.
        /// </param>
        /// <param name="rev">
        /// The revision number.
        /// </param>
        public FileRevision(Guid iid, int rev) : base(iid: iid, rev: rev)
        {
            this.FileType = new List<OrderedItem>();
        }

        /// <summary>
        /// Gets or sets the unique identifier of the referenced ContainingFolder.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: true, isPersistent: true)]
        [DataMember]
        public Guid? ContainingFolder { get; set; }

        /// <summary>
        /// Gets or sets the ContentHash.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        [DataMember]
        public string ContentHash { get; set; }

        /// <summary>
        /// Gets or sets the CreatedOn.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        [DataMember]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the referenced Creator.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        [DataMember]
        public Guid Creator { get; set; }

        /// <summary>
        /// Gets or sets the list of ordered unique identifiers of the referenced FileType instances.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: true, isNullable: false, isPersistent: true)]
        [DataMember]
        public List<OrderedItem> FileType { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The Path property is a derived property; when the getter and setter are invoked an InvalidOperationException will be thrown.
        /// </exception>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: true, isOrdered: false, isNullable: false, isPersistent: false)]
        [XmlIgnore]
        public string Path
        {
            get { throw new InvalidOperationException("Forbidden Get value for the derived property FileRevision.Path"); }
            set { throw new InvalidOperationException("Forbidden Set value for the derived property FileRevision.Path"); }
        }

        /// <summary>
        /// Gets the route for the current <see ref="FileRevision"/>.
        /// </summary>
        public override string Route
        {
            get { return this.ComputedRoute(); }
        }

        /// <summary>
        /// Instantiate a <see cref="CDP4Common.EngineeringModelData.FileRevision"/> from a <see cref="FileRevision"/>
        /// </summary>
        /// <param name="cache">The cache that stores all the <see cref="CommonData.Thing"/>s</param>.
        /// <param name="uri">The <see cref="Uri"/> of the <see cref="CDP4Common.EngineeringModelData.FileRevision"/></param>.
        /// <returns>A new <see cref="CommonData.Thing"/></returns>
        public override CommonData.Thing InstantiatePoco(ConcurrentDictionary<CacheKey, Lazy<CommonData.Thing>> cache, Uri uri)
        {
            return new CDP4Common.EngineeringModelData.FileRevision(this.Iid, cache, uri);
        }

        /// <summary>
        /// Resolves the properties of a copied <see cref="Thing"/> based on the original and a collection of copied <see cref="Thing"/>.
        /// </summary>
        /// <param name="originalThing">The original <see cref="Thing"/></param>.
        /// <param name="originalCopyMap">The map containig all instance of copied <see cref="Thing"/>s with their original</param>.
        public override void ResolveCopy(Thing originalThing, IReadOnlyDictionary<Thing, Thing> originalCopyMap)
        {
            var original = originalThing as FileRevision;
            if (original == null)
            {
                throw new InvalidOperationException("The originalThing cannot be null or is of the incorrect type");
            }

            var copyContainingFolder = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == original.ContainingFolder);
            this.ContainingFolder = copyContainingFolder.Value == null ? original.ContainingFolder : copyContainingFolder.Value.Iid;

            this.ContentHash = original.ContentHash;

            this.CreatedOn = original.CreatedOn;

            var copyCreator = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == original.Creator);
            this.Creator = copyCreator.Value == null ? original.Creator : copyCreator.Value.Iid;

            foreach (var guid in original.ExcludedDomain)
            {
                var copy = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == guid);
                this.ExcludedDomain.Add(copy.Value == null ? guid : copy.Value.Iid);
            }

            foreach (var guid in original.ExcludedPerson)
            {
                var copy = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == guid);
                this.ExcludedPerson.Add(copy.Value == null ? guid : copy.Value.Iid);
            }

            foreach (var orderedItem in original.FileType)
            {
                var copy = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == Guid.Parse(orderedItem.V.ToString()));
                this.FileType.Add(new OrderedItem { K = orderedItem.K, V = copy.Value == null ? orderedItem.V : copy.Value.Iid });
            }

            this.ModifiedOn = original.ModifiedOn;

            this.Name = original.Name;

            this.ThingPreference = original.ThingPreference;
        }

        /// <summary>
        /// Resolves the references of a copied <see cref="Thing"/> based on a original to copy map.
        /// </summary>
        /// <param name="originalCopyMap">The map containig all instance of copied <see cref="Thing"/>s with their original</param>.
        /// <returns>True if a modification was done in the process of this method</returns>.
        public override bool ResolveCopyReference(IReadOnlyDictionary<Thing, Thing> originalCopyMap)
        {
            var hasChanges = false;

            var copyCreator = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == this.Creator);
            if (copyCreator.Key != null)
            {
                this.Creator = copyCreator.Value.Iid;
                hasChanges = true;
            }

            for (var i = 0; i < this.FileType.Count; i++)
            {
                var copy = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == Guid.Parse(this.FileType[i].V.ToString()));
                if (copy.Key != null)
                {
                    this.FileType[i].V = copy.Value.Iid;
                    hasChanges = true;
                }
            }

            return hasChanges;
        }
    }
}
