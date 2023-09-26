using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Services.Domain.CourseServices.Contracts;

namespace CourseHub.Core.Services.Domain.CourseServices;

public class EnrollmentService : DomainService, IEnrollmentService
{
    public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<ServiceResult<bool>> IsEnrolled(Guid courseId, Guid client)
    {
        bool result = await _uow.EnrollmentRepo.IsEnrolled(client, courseId);
        return ToQueryResult(result);
    }

    public Task<ServiceResult> Enroll(Guid courseId, Guid client, Guid billId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> Unenroll(Guid courseId, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task ForceCommitAsync()
    {
        throw new NotImplementedException();
    }
}
