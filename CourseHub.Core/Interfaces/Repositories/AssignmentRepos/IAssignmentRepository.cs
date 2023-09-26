using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Assignment.AssignmentModels;

namespace CourseHub.Core.Interfaces.Repositories.AssignmentRepos;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<AssignmentModel> GetAsync(Guid id);
    Task<AssignmentMinModel> GetMinAsync(Guid id);
}
