// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonAccessRightKind.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated Enumeration. Any manual changes to this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.CommonData
{
    /// <summary>
    /// enumeration datatype that defines the possible access rights for a class of objects in a PersonPermissionNote 1: The relevant class of objects is specified in the <i>objectClass</i> property of a PersonPermission.Note 2: PersonAccessRightKind.MODIFY implies read, create and update actions, and if delete is allowed, deleted actions as well.Note 3: PersonAccessRightKind.MODIFY access to an object also implies the right to modify the container collection that contains the object, even if the access right to the container collection object is limited to PersonAccessRightKind.READ.
    /// </summary>
    public enum PersonAccessRightKind
    {
        /// <summary>
        /// assertion that Person access to the given class of objects is not applicable
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
        /// read-only access to information contained in EngineeringModelSetups where the authenticated Person is a ParticipantNote: If an authenticated Person has PersonAccessRightKind.MODIFY_OWN_PERSON to the ClassKind.Person, then READ_IF_PARTICIPANT also implies READ access on any Person that is associated with any Participant in any of the EngineeringModelSetups in which the authenticated Person is a Participant. In other words, READ access to the union of Persons referenced by Participants in the union of EngineeringModelSetups for which the authenticated Person has at least READ_IF_PARTICIPANT access. Basically this means that a Participant has access to the information describing the other Participants and Persons in a team that he or she is a member of.
        /// </summary>
        READ_IF_PARTICIPANT,

        /// <summary>
        /// modify access to information contained in EngineeringModelSetups where the authenticated Person is a Participant
        /// </summary>
        MODIFY_IF_PARTICIPANT,

        /// <summary>
        /// modify access to the Person data of the actual authenticated person (i.e. user) in a session
        /// </summary>
        MODIFY_OWN_PERSON,
    }
}