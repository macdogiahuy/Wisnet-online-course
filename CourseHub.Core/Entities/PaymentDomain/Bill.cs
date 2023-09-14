using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.UserDomain;

namespace CourseHub.Core.Entities.PaymentDomain;

#pragma warning disable CS8618

public class Bill : AuditedEntity
{
    // Attributes
    public string Action { get; set; }                      // System Action
    public string Note { get; set; }
    public string Amount { get; set; }
    public string Gateway { get; set; }
    public string TransactionId { get; set; }
    public string ClientTransactionId { get; set; }
    public string Token { get; set; }
    public bool IsSuccessful { get; set; }

    // Navigations
    public User? Creator { get; set; }
}

#pragma warning restore CS8618