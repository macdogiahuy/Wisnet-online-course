using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.RequestDtos.Course.InstructorDtos;

namespace CourseHub.Core.Services.Domain.CourseServices;

public interface IInstructorService
{
    Task<ServiceResult<Guid>> CreateAsync(CreateInstructorDto dto, Guid creatorId);
    Task<ServiceResult> GetAsync(QueryInstructorDto dto);
    Task<ServiceResult> UpdateAsync(UpdateInstructorDto dto);
}
