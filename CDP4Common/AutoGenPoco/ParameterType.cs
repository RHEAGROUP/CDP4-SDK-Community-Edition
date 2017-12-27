// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterType.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
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
    /// abstract superclass that represents the common characteristics of any parameter type
    /// Note: There are two properties to hold a short identifier to designate a ParameterType: <i>shortName</i> and <i>symbol</i>. The <i>shortName</i> must be case-insensitive unique within its containing ReferenceDataLibrary. This is necessary in order to support case-insensitive unique names in derived parameter names for use in modeling environments that only have case-insensitive variable names such as MS Excel. The <i>symbol</i> must be case-sensitive unique within its containing ReferenceDataLibrary. The <i>symbol</i> is meant to hold the official symbolic name of a ParameterType as defined for example in an ISO standard.
    /// </summary>
    [Container(typeof(ReferenceDataLibrary), "ParameterType")]
    public abstract partial class ParameterType : DefinedThing, ICategorizableThing, IDeprecatableThing
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
        /// Initializes a new instance of the <see cref="ParameterType"/> class.
        /// </summary>
        protected ParameterType()
        {
            this.Category = new List<Category>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterType"/> class.
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
        protected ParameterType(Guid iid, ConcurrentDictionary<Tuple<Guid, Guid?>, Lazy<Thing>> cache, Uri iDalUri) : base(iid, cache, iDalUri)
        {
            this.Category = new List<Category>();
        }

        /// <summary>
        /// Gets or sets a list of Category.
        /// </summary>
        /// <remarks>
        /// reference to zero or more Categories of which this CategorizableThing is a member
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public virtual List<Category> Category { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDeprecated.
        /// </summary>
        /// <remarks>
        /// assertion whether a DeprecatableThing is deprecated or not
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public virtual bool IsDeprecated { get; set; }

        /// <summary>
        /// Gets or sets the NumberOfValues.
        /// </summary>
        /// <remarks>
        /// number of individual values in each of the parameter value properties of a ParameterValueSet, a ParameterSubscriptionValueSet or a SimpleParameterValue for this ParameterType
        /// Note: For a ScalarParameterType this will be one, while for a CompoundParameterType this will amount to the (possibly recursive) summation of the <i>numberOfValues</i> in the ParameterTypes of all <i>component</i> ParameterTypeComponents.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The NumberOfValues property is a derived property; when the getter and setter are invoked an InvalidOperationException will be thrown.
        /// </exception>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: true, isOrdered: false, isNullable: false, isPersistent: false)]
        public int NumberOfValues
        {
            get { return this.GetDerivedNumberOfValues(); }
            set { throw new InvalidOperationException("Forbidden Set value for the derived property ParameterType.NumberOfValues"); }
        }

        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        /// <remarks>
        /// short symbolic name of this ParameterType
        /// Note: Where applicable this property shall be used to hold the symbol that is defined through a standard (e.g. from ISO) or by convention.
        /// </remarks>
        [UmlInformation(aggregation: AggregationKind.None, isDerived: false, isOrdered: false, isNullable: false, isPersistent: true)]
        public virtual string Symbol { get; set; }

        /// <summary>
        /// Creates and returns a copy of this <see cref="ParameterType"/> for edit purpose.
        /// </summary>
        /// <param name="cloneContainedThings">A value that indicates whether the contained <see cref="Thing"/>s should be cloned or not.</param>
        /// <returns>
        /// A cloned instance of <see cref="ParameterType"/>.
        /// </returns>
        public new ParameterType Clone(bool cloneContainedThings)
        {
            this.ChangeKind = ChangeKind.Update;
            return (ParameterType)this.GenericClone(cloneContainedThings);
        }

        /// <summary>
        /// Validates the cardinalities of the properties of this <see cref="ParameterType"/>.
        /// </summary>
        /// <returns>
        /// A list of potential errors.
        /// </returns>
        protected override IEnumerable<string> ValidatePocoCardinality()
        {
            var errorList = new List<string>(base.ValidatePocoCardinality());

            if (string.IsNullOrWhiteSpace(this.Symbol))
            {
                errorList.Add("The property Symbol is null or empty.");
            }

            return errorList;
        }
    }
}