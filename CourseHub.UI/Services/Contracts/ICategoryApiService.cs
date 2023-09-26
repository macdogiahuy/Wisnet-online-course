using CourseHub.Core.Entities.CourseDomain;

namespace CourseHub.UI.Services.Contracts;

public interface ICategoryApiService
{
    public Task<List<Category>> GetAsync();
}
