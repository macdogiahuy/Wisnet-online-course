using CourseHub.Core.Entities.AssignmentDomain;
using CourseHub.Core.Interfaces.Repositories.AssignmentRepos;
using CourseHub.Core.Models.Assignment.AssignmentModels;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.AssignmentRepos;

public class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(DbContext context) : base(context)
    {
    }

    public Task<AssignmentModel> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<AssignmentMinModel> GetMinAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
