using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.CourseRepos;

public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(DbContext context) : base(context)
    {
    }

    public async Task<bool> IsEnrolled(Guid userId, Guid courseId)
    {
        return await DbSet.AnyAsync(_ => _.CreatorId == userId && _.CourseId == courseId);
    }
}
