using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Social;
using CourseHub.Core.RequestDtos.Social.ChatMessageDtos;

namespace CourseHub.Core.Services.Domain.SocialServices.Contracts;

public interface IChatMessageService
{
    Task<ServiceResult<PagedResult<ChatMessageModel>>> Get(QueryChatMessageDto dto, Guid client);

    Task<ServiceResult<Guid>> Create(CreateChatMessageDto dto, Guid client);
    Task<ServiceResult> Update(UpdateChatMessageDto dto, Guid client);
    Task<ServiceResult> Delete(Guid id, Guid client);
}
