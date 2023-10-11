using CourseHub.Core.Interfaces.Repositories.Shared;

namespace CourseHub.Core.Interfaces.Repositories.CourseRepos;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<bool> IsEnrolled(Guid userId, Guid courseId);
}