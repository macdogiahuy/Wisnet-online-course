using CourseHub.Core.Entities.CourseDomain;

namespace CourseHub.UI.Services.Contracts.CourseServices;

public interface ICategoryApiService
{
    public Task<List<Category>> GetAsync();
}
