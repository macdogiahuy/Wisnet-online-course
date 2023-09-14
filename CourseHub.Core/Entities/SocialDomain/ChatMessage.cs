using CourseHub.Core.Entities.CommonDomain;
using CourseHub.Core.Entities.Contracts;
using CourseHub.Core.Entities.SocialDomain.Enums;
using CourseHub.Core.Entities.UserDomain;

namespace CourseHub.Core.Entities.SocialDomain;

public class ChatMessage : AuditedEntity
{
	// Attributes
	public string Content { get; set; }
	public MessageStatus Status { get; set; }

	// FKs
	public Guid ConversationId { get; set; }

	// Navigations
	public User? Creator { get; set; }
	public Conversation? Conversation { get; set; }
	public ICollection<Reaction> Reactions { get; set; }
	// ICollection<Attachment>
}
