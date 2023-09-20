using CourseHub.Core.Entities.UserDomain.Enums;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Common.NotificationModels;
using CourseHub.Core.RequestDtos.Common.NotificationDtos;

namespace CourseHub.Core.Services.Domain.CommonServices;

public interface INotificationService
{
    public Task<ServiceResult<PagedResult<NotificationModel>>> GetPagedAsync(QueryNotificationDto dto);
    public Task<ServiceResult<Guid>> CreateAsync(CreateNotificationDto dto, Guid? client);
    public Task<ServiceResult> UpdateAsync(UpdateNotificationDto dto, Guid? client);
}