// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependentParameterTypeAssignment.cs" company="RHEA System S.A.">
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
    /// A Data Transfer Object representation of the <see cref="DependentParameterTypeAssignment"/> class.
    /// </summary>
    [DataContract]
    [CDPVersion("1.2.0")]
    [Container(typeof(SampledFunctionParameterType), "DependentParameterType")]
    public sealed partial class DependentParameterTypeAssignment : Thing, IParameterTypeAssignment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentParameterTypeAssignment"/> class.
        /// </summary>
        public DependentParameterTypeAssignment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentParameterTypeAssignment"/> class.
        /// </summary>
        /// <param name="iid">
        /// The unique identifier.
        /// </param>
        /// <param name="rev">
        /// The revision number.
        /// </param>
        public DependentParameterTypeAssignment(Guid iid, int rev) : base(iid: iid, rev: rev)
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the referenced MeasurementScale.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: true, isPersistent: true)]
        [DataMember]
        public Guid? MeasurementScale { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the referenced ParameterType.
        /// </summary>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        [DataMember]
        public Guid ParameterType { get; set; }

        /// <summary>
        /// Gets the route for the current <see ref="DependentParameterTypeAssignment"/>.
        /// </summary>
        public override string Route
        {
            get { return this.ComputedRoute(); }
        }

        /// <summary>
        /// Instantiate a <see cref="CDP4Common.SiteDirectoryData.DependentParameterTypeAssignment"/> from a <see cref="DependentParameterTypeAssignment"/>
        /// </summary>
        /// <param name="cache">The cache that stores all the <see cref="CommonData.Thing"/>s</param>.
        /// <param name="uri">The <see cref="Uri"/> of the <see cref="CDP4Common.SiteDirectoryData.DependentParameterTypeAssignment"/></param>.
        /// <returns>A new <see cref="CommonData.Thing"/></returns>
        public override CommonData.Thing InstantiatePoco(ConcurrentDictionary<CacheKey, Lazy<CommonData.Thing>> cache, Uri uri)
        {
            return new CDP4Common.SiteDirectoryData.DependentParameterTypeAssignment(this.Iid, cache, uri);
        }

        /// <summary>
        /// Resolves the properties of a copied <see cref="Thing"/> based on the original and a collection of copied <see cref="Thing"/>.
        /// </summary>
        /// <param name="originalThing">The original <see cref="Thing"/></param>.
        /// <param name="originalCopyMap">The map containig all instance of copied <see cref="Thing"/>s with their original</param>.
        public override void ResolveCopy(Thing originalThing, IReadOnlyDictionary<Thing, Thing> originalCopyMap)
        {
            var original = originalThing as DependentParameterTypeAssignment;
            if (original == null)
            {
                throw new InvalidOperationException("The originalThing cannot be null or is of the incorrect type");
            }

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

            var copyMeasurementScale = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == original.MeasurementScale);
            this.MeasurementScale = copyMeasurementScale.Value == null ? original.MeasurementScale : copyMeasurementScale.Value.Iid;

            this.ModifiedOn = original.ModifiedOn;

            var copyParameterType = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == original.ParameterType);
            this.ParameterType = copyParameterType.Value == null ? original.ParameterType : copyParameterType.Value.Iid;

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

            var copyMeasurementScale = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == this.MeasurementScale);
            if (copyMeasurementScale.Key != null)
            {
                this.MeasurementScale = copyMeasurementScale.Value.Iid;
                hasChanges = true;
            }

            var copyParameterType = originalCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == this.ParameterType);
            if (copyParameterType.Key != null)
            {
                this.ParameterType = copyParameterType.Value.Iid;
                hasChanges = true;
            }

            return hasChanges;
        }
    }
}
