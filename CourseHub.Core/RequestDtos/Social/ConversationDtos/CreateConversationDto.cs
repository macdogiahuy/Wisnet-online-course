﻿namespace CourseHub.Core.RequestDtos.Social.ConversationDtos;

public class CreateConversationDto
{
    public string? Title { get; set; }
    public List<Guid> OtherParticipants { get; set; }
}
