// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantAccessRightKind.cs" company="RHEA System S.A.">
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
//   This is an auto-generated Enumeration. Any manual changes to this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.CommonData
{
    /// <summary>
    /// enumeration datatype that defines the possible access rights for a class of objects in a ParticipantPermissionNote 1: The relevant class of objects is specified in the <i>objectClass</i> property of a ParticipantPermission.Note 2: ParticipantAccessRightKind.MODIFY implies read, create and update actions, and if delete is allowed, deleted actions as well.Note 3: ParticipantAccessRightKind.MODIFY access to an object also implies the right to modify the container collection that contains the object, even if the access right to the container collection object is limited to ParticipantAccessRightKind.READ.
    /// </summary>
    public enum ParticipantAccessRightKind
    {
        /// <summary>
        /// assertion that Participant access to the given class of objects is not applicable
        /// </summary>
        NOT_APPLICABLE,

        /// <summary>
        /// no access
        /// </summary>
        NONE,

        /// <summary>
        /// access right to a given class of objects is the same as that of the class of its container object
        /// </summary>
        SAME_AS_CONTAINER,

        /// <summary>
        /// access right to a given class of objects is the same as that of its superclass
        /// </summary>
        SAME_AS_SUPERCLASS,

        /// <summary>
        /// read-only access
        /// </summary>
        READ,

        /// <summary>
        /// modify access
        /// </summary>
        MODIFY,

        /// <summary>
        /// modify access for objects owned by a selected DomainOfExpertiseNote: A Participant in a concurrent engineering session can only represent one DomainOfExpertise at a time, but may switch between the (possible) domains assigned to the Participant.
        /// </summary>
        MODIFY_IF_OWNER,
    }
}
