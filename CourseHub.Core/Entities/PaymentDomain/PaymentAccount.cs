﻿using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.UserDomain;

namespace CourseHub.Core.Entities.PaymentDomain;

#pragma warning disable CS8618

public class PaymentAccount : AuditedEntity
{
    // Attributes
    public string Gateway { get; set; }
    public string AccountNumber { get; set; }
    public string AccountHolderName { get; set; }

    // Navigations
    public User? Creator { get; set; }
}

#pragma warning restore CS8618