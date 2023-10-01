using Microsoft.AspNetCore.Http;

namespace CourseHub.Core.RequestDtos.Social.ChatMessageDtos;

public class CreateChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }

    public string Content { get; set; }
    public List<IFormFile>? Attachments { get; set; }
}
