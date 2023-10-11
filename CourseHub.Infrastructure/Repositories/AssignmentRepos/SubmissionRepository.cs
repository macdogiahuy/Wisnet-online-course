using CourseHub.Core.Entities.AssignmentDomain;
using CourseHub.Core.Interfaces.Repositories.AssignmentRepos;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.AssignmentRepos;

public class SubmissionRepository : BaseRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(DbContext context) : base(context)
    {
    }
}
