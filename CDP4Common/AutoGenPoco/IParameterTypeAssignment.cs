// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterTypeAssignment.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Authors: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Yevhen Ikonnykov, Smiechowski Nathanael
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
//   This is an auto-generated POCO Interface. Any manual changes to this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.SiteDirectoryData
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    /// <summary>
    /// a pairing of an assigned ParameterType and a MeasurementScale in case a QuantityKind is selected. The MeasurementScale must be in the array of possible MeasurementScales of the the ParameterType.
    /// </summary>
    public partial interface IParameterTypeAssignment
    {
        /// <summary>
        /// Gets or sets the MeasurementScale.
        /// </summary>
        /// <remarks>
        /// the MeasurementScale applicable to the ParameterType of the selection. Null if the ParameterType is not a QuantityKind. Must be in the array of possible MeasurementScales of the selected ParameterType.
        /// </remarks>
        MeasurementScale MeasurementScale { get; set; }

        /// <summary>
        /// Gets or sets the ParameterType.
        /// </summary>
        /// <remarks>
        /// the selected ParameterType assignment.
        /// </remarks>
        ParameterType ParameterType { get; set; }
    }
}
