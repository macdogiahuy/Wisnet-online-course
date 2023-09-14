using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Interfaces.Repositories.Shared;

namespace CourseHub.Core.Interfaces.Repositories.CourseRepos;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllAsync();
}
