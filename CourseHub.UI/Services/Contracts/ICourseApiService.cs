using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;

namespace CourseHub.UI.Services.Contracts;

public interface ICourseApiService
{
    Task<CourseModel?> GetAsync(Guid id);
    Task<PagedResult<CourseOverviewModel>> GetPagedAsync(QueryCourseDto dto);
    Task<List<CourseOverviewModel>?> GetMultipleAsync(Guid[] ids);
    Task<List<CourseOverviewModel>?> GetSimilarAsync(Guid id);
    Task<List<CourseMinModel>?> GetMinAsync(QueryCourseDto id);

    Task<HttpResponseMessage> CreateAsync(CreateCourseDto dto, HttpContext context);
    Task<HttpResponseMessage> UpdateAsync(UpdateCourseDto dto, HttpContext context);
    Task<HttpResponseMessage> DeleteAsync(Guid id, HttpContext context);

    Task<bool> IsEnrolled(Guid courseId);
}