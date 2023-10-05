using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Helpers.Text;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Social;
using CourseHub.Core.RequestDtos.Social.ConversationDtos;
using CourseHub.Core.Services.Domain.SocialServices.Contracts;
using System.Linq.Expressions;

namespace CourseHub.Core.Services.Domain.SocialServices;

public class ConversationService : DomainService, IConversationService
{
    public ConversationService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }



    public async Task<ServiceResult<ConversationModel>> Get(Guid id, Guid client)
    {
        var result = await _uow.ConversationRepo.GetAsync(id);

        if (result is null)
            return NotFound<ConversationModel>();
        if (!result.Members.Any(_ => _.CreatorId == client))
            return Unauthorized<ConversationModel>();

        return ToQueryResult(result);
    }

    public async Task<ServiceResult<PagedResult<ConversationModel>>> Get(QueryConversationDto dto, Guid client)
    {
        var query = _uow.ConversationRepo.GetPagingQuery(GetPredicate(dto, client), dto.PageIndex, dto.PageSize);

        PagedResult<ConversationModel> result = await query.ExecuteWithOrderBy(_ => _.CreationTime);

        return ToQueryResult(result);
    }



    public async Task<ServiceResult<Guid>> Create(CreateConversationDto dto, Guid client)
    {
        // request.OtherParticipants -> AllParticipants
        dto.OtherParticipants.Add(client);
        dto.OtherParticipants = dto.OtherParticipants.Distinct().ToList();
        if (dto.OtherParticipants.Count < 2)
            return BadRequest<Guid>(ConversationDomainMessages.INVALID_PARTICIPANTS);

        var entity = await Adapt(dto, client);
        await _uow.ConversationRepo.Insert(entity);
        await _uow.CommitAsync();
        return Created(entity.Id);
    }

    public async Task<ServiceResult> Update(UpdateConversationDto dto, Guid client)
    {
        var entity = await _uow.ConversationRepo.FindWithMembers(dto.Id);
        if (entity is null)
            return BadRequest();

        var clientMember = entity.Members.FirstOrDefault(_ => _.CreatorId == client);
        //...
        if (clientMember is null)
            return Unauthorized();

        try
        {
            await ApplyChanges(dto, entity);
            await _uow.CommitAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return ServerError();
        }
    }

    public async Task<ServiceResult> Delete(Guid id, Guid client)
    {
        var entity = await _uow.ConversationRepo.FindWithMembers(id);
        if (entity is null)
            return BadRequest();
        var clientMember = entity.Members.FirstOrDefault(_ => _.CreatorId == client);
        if (clientMember is null || !clientMember.IsAdmin)
            return Unauthorized();

        _uow.ConversationRepo.Delete(entity);
        await _uow.CommitAsync();
        return Ok();
    }





    private async Task<Conversation> Adapt(CreateConversationDto _, Guid client)
    {
        bool isPrivate = _.OtherParticipants.Count == 2;

        Guid id = Guid.NewGuid();
        string avatarUrl = string.Empty;
        List<ConversationMember> members = _.OtherParticipants
            .Select(_ => new ConversationMember(id, _, _ == client))
            .ToList();



        // request.OtherParticipants == AllParticipants
        Conversation entity = new(
            id,
            client,
            isPrivate || _.Title is null ? string.Empty : _.Title,
            isPrivate,
            avatarUrl,
            members
        );

        return entity;
    }






    private Expression<Func<Conversation, bool>>? GetPredicate(QueryConversationDto dto, Guid client)
    {
        if (dto.ByClient)
            return _ => _.Members.Any(_ => _.CreatorId == client);

        //...
        if (dto.Title is not null)
            return _ => _.Title.Contains(TextHelper.Normalize(dto.Title));

        // Where Contains
        //dto.ConversationIds
        return null;
    }

    private async Task ApplyChanges(UpdateConversationDto dto, Conversation entity)
    {

    }
}
