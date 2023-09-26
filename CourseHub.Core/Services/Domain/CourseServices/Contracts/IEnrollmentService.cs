using CourseHub.Core.Helpers.Messaging;

namespace CourseHub.Core.Services.Domain.CourseServices.Contracts;

public interface IEnrollmentService
{
    Task<ServiceResult<bool>> IsEnrolled(Guid courseId, Guid client);

    /// <summary>
    /// Not commited operation
    /// </summary>
    Task<ServiceResult> Enroll(Guid courseId, Guid client, Guid billId);

    /// <summary>
    /// Update the Enrollment
    /// </summary>
    Task<ServiceResult> Unenroll(Guid courseId, Guid client);

    Task ForceCommitAsync();
}
