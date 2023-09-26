using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.RequestDtos.Course.InstructorDtos;
using CourseHub.Core.Services.Domain.CourseServices.Contracts;

namespace CourseHub.Core.Services.Domain.CourseServices;

public class InstructorService : DomainService, IInstructorService
{
    public InstructorService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }



    public Task<ServiceResult> GetAsync(QueryInstructorDto dto)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Not committed
    /// </summary>
    /*public async Task<ServiceResult<Guid>> CreateAsync(CreateInstructorDto dto, Guid creatorId)
    {
        var entity = Adapt(dto, creatorId);
        await _uow.InstructorRepo.Insert(entity);
        return Created(entity.Id);
    }*/

    public Task<ServiceResult> UpdateAsync(UpdateInstructorDto dto)
    {
        throw new NotImplementedException();
    }






    /*private Instructor Adapt(CreateInstructorDto dto, Guid creatorId)
    {
        return new Instructor(Guid.NewGuid(), dto.Intro, dto.Experience, creatorId);
    }*/
}
