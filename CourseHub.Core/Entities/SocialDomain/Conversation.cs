using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.UserDomain;

namespace CourseHub.Core.Entities.SocialDomain;

public class Conversation : AuditedEntity
{
    // Attributes
    public string Title { get; set; }
    public bool IsPrivate { get; set; }
    public string AvatarUrl { get; set; }

    // Navigations
    public User? Creator { get; set; }
    public ICollection<ConversationMember> Members { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; }
}
