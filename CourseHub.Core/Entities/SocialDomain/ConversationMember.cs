﻿using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.UserDomain;

namespace CourseHub.Core.Entities.SocialDomain;

public class ConversationMember : CreationAuditedDomainObject
{
    // Keys (with CreatorId from base)
    public Guid ConversationId { get; set; }

    // Attributes
    public bool IsAdmin { get; set; }
    public DateTime LastVisit { get; set; }             // Not LastModification

    // Navigations
    public User? Creator { get; set; }
    public Conversation? Conversation { get; set; }
}
