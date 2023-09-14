using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.CourseDomain.Enums;
using CourseHub.Core.Entities.PaymentDomain;

namespace CourseHub.Core.Entities.CourseDomain;

public class Enrollment : CreationAuditedDomainObject
{
    // Keys (with CreatorId from base)
    public Guid CourseId { get; set; }

    // Attributes
    public CourseStatus Status { get; set; }

    // FKs
    public Guid? BillId { get; set; }

    // Navigation
    public Bill? Bill { get; set; }
}
