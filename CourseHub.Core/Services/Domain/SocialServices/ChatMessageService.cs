using AutoMapper;
using CourseHub.Core.Entities.SocialDomain.Enums;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Social;
using CourseHub.Core.RequestDtos.Social.ChatMessageDtos;
using CourseHub.Core.RequestDtos.Social.ConversationDtos;
using CourseHub.Core.Services.Domain.SocialServices.Contracts;
using System.Linq.Expressions;

namespace CourseHub.Core.Services.Domain.SocialServices;

public class ChatMessageService : DomainService, IChatMessageService
{
    public ChatMessageService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }



    public async Task<ServiceResult<PagedResult<ChatMessageModel>>> Get(QueryChatMessageDto dto, Guid client)
    {
        var conversation = await _uow.ConversationRepo.Find(dto.ConversationId);
        if (conversation is null)
            return BadRequest<PagedResult<ChatMessageModel>>(ConversationDomainMessages.INVALID_CONVERSATION);
        if (!conversation.Members.Any(_ => _.CreatorId == client))
            return Unauthorized<PagedResult<ChatMessageModel>>();

        var query = _uow.ChatMessageRepo.GetPagingQuery(GetPredicate(dto, client), dto.PageIndex, dto.PageSize);

        PagedResult<ChatMessageModel> result = await query.ExecuteWithOrderBy(_ => _.CreationTime);

        return ToQueryResult(result);
    }



    public async Task<ServiceResult<Guid>> Create(CreateChatMessageDto dto, Guid client)
    {
        var conversation = await _uow.ConversationRepo.Find(dto.ConversationId);
        if (conversation is null)
            return BadRequest<Guid>(ConversationDomainMessages.INVALID_CONVERSATION);
        if (!conversation.Members.Any(_ => _.CreatorId == client))
            return Unauthorized<Guid>();

        var entity = Adapt(dto, client);
        await _uow.ChatMessageRepo.Insert(entity);
        await _uow.CommitAsync();
        return Created(entity.Id);
    }

    public Task<ServiceResult> Update(UpdateChatMessageDto dto, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> Delete(Guid id, Guid client)
    {
        throw new NotImplementedException();
    }






    private Expression<Func<ChatMessage, bool>>? GetPredicate(QueryChatMessageDto dto, Guid client)
    {
        return _ => _.ConversationId == dto.ConversationId;
    }

    private ChatMessage Adapt(CreateChatMessageDto _, Guid client)
    {
        Guid id = Guid.NewGuid();
        return new ChatMessage(id, client, _.Content, _.ConversationId);
    }
}
