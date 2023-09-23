using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;

namespace CourseHub.Core.Services.Domain.CourseServices;

public interface ICourseService
{
    Task<ServiceResult<CourseModel>> GetAsync(Guid id);
    Task<ServiceResult<PagedResult<CourseOverviewModel>>> GetPagedAsync(QueryCourseDto dto);
    Task<ServiceResult<List<CourseOverviewModel>>> GetMultipleAsync(Guid[] ids);
    Task<ServiceResult<List<CourseOverviewModel>>> GetSimilarAsync(Guid id);
    Task<ServiceResult<List<CourseMinModel>>> GetMinAsync(QueryCourseDto id);

    Task<ServiceResult<Guid>> CreateAsync(CreateCourseDto dto, Guid client);
    Task<ServiceResult> UpdateAsync(UpdateCourseDto dto, Guid client);
    Task<ServiceResult> DeleteAsync(Guid id, Guid client);
}
