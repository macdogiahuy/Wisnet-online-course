using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Models.Assignment.SubmissionModels;
using CourseHub.Core.RequestDtos.Assignment.SubmissionDtos;

namespace CourseHub.Core.Services.Domain.AssignmentServices.Contracts;

public interface ISubmissionService
{
    Task<ServiceResult<SubmissionModel>> GetAsync(Guid id);
    Task<ServiceResult<SubmissionMinModel>> GetMinAsync(Guid id);

    Task<ServiceResult<Guid>> CreateAsync(CreateSubmissionDto dto, Guid client);
    Task<ServiceResult> UpdateAsync(UpdateSubmissionDto dto, Guid client);
    Task<ServiceResult> DeleteAsync(Guid id, Guid client);
}