using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Common.NotificationModels;
using CourseHub.Core.RequestDtos.Common.NotificationDtos;
using System.Linq.Expressions;
using CourseHub.Core.Entities.CommonDomain.Enums;
using CourseHub.Core.Entities.UserDomain.Enums;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Services.Domain.CommonServices.Contracts;

namespace CourseHub.Core.Services.Domain.CommonServices;

public class NotificationService : DomainService, INotificationService
{
    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<ServiceResult<Guid>> CreateAsync(CreateNotificationDto dto, Guid? client)
    {
        if (client is null)
            return Unauthorized<Guid>();

        var entity = Adapt(dto, (Guid)client);
        await _uow.NotificationRepo.Insert(entity);
        await _uow.CommitAsync();
        return Created(entity.Id);
    }

    public async Task<ServiceResult<PagedResult<NotificationModel>>> GetPagedAsync(QueryNotificationDto dto)
    {
        var query = _uow.NotificationRepo.GetPagingQuery(GetPredicate(dto), dto.PageIndex, dto.PageSize);
        var result = await query.ExecuteWithOrderBy(_ => _.CreationTime);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateNotificationDto dto, Guid? client)
    {
        if (client is null)
            return Unauthorized();

        var entity = await _uow.NotificationRepo.Find(dto.Id);
        if (entity is null)
            return BadRequest();

        //... outer
        var user = await _uow.UserRepo.Find(client);
        if (user is null)
            return Unauthorized();
        if (entity.Type == NotificationType.RequestToBecomeInstructor && user.Role < Role.Admin)
            return Unauthorized();
        try
        {
            await ApplyChanges(dto, entity);
            await _uow.CommitAsync();
        }
        catch (Exception ex)
        {
            return ServerError();
        }
        return Ok();
    }






    private Notification Adapt(CreateNotificationDto dto, Guid client)
    {
        return new Notification(client, dto.Message, dto.Type, dto.ReceiverId);
    }

    private async Task ApplyChanges(UpdateNotificationDto dto, Notification entity)
    {
        if (entity.Type == NotificationType.RequestToBecomeInstructor && dto.Status == NotificationStatus.Approved)
        {
            var user = await _uow.UserRepo.Find(entity.CreatorId);
            if (user is null)
                throw new Exception(UserDomainMessages.NOT_FOUND);
            Guid instructorId = Guid.NewGuid();
            await _uow.InstructorRepo.Insert(new Instructor(instructorId, user.Id));
            user.SetInstructor(instructorId);
        }
        entity.Status = dto.Status;
    }

    private Expression<Func<Notification, bool>>? GetPredicate(QueryNotificationDto dto)
    {
        if (dto.CreatorId is not null)
            return _ => _.CreatorId == dto.CreatorId;
        if (dto.ReceiverId is not null)
            return _ => _.ReceiverId == dto.ReceiverId;
        if (dto.Type is not null)
            return _ => _.Type == dto.Type;
        if (dto.Status is not null)
            return _ => _.Status == dto.Status;
        return null;
    }
}
