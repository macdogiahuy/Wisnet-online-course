using AutoMapper;
using CourseHub.Core.Models.Social;

namespace CourseHub.Core.Services.Mappers.ConversationMappers;

public class ChatMessageMapperProfile : Profile
{
    public static readonly IConfigurationProvider ModelConfig = new MapperConfiguration(
        cfg =>
        {
            cfg.CreateMap<ChatMessage, ChatMessageModel>();
        }
    );
}
