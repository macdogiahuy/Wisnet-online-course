using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Models.Assignment.SubmissionModels;
using CourseHub.Core.RequestDtos.Assignment.SubmissionDtos;
using CourseHub.Core.Services.Domain.AssignmentServices.Contracts;

namespace CourseHub.Core.Services.Domain.AssignmentServices;

public class SubmissionService : DomainService, ISubmissionService
{
    public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }

    public Task<ServiceResult<Guid>> CreateAsync(CreateSubmissionDto dto, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(Guid id, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<SubmissionModel>> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<SubmissionMinModel>> GetMinAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> UpdateAsync(UpdateSubmissionDto dto, Guid client)
    {
        throw new NotImplementedException();
    }
}
