using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Models.Assignment.AssignmentModels;
using CourseHub.Core.RequestDtos.Assignment.AssignmentDtos;
using CourseHub.Core.Services.Domain.AssignmentServices.Contracts;

namespace CourseHub.Core.Services.Domain.AssignmentServices;

public class AssignmentService : DomainService, IAssignmentService
{
    public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }



    public async Task<ServiceResult<AssignmentModel>> GetAsync(Guid id)
    {
        var result = await _uow.AssignmentRepo.GetAsync(id);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<AssignmentMinModel>> GetMinAsync(Guid id)
    {
        var result = await _uow.AssignmentRepo.GetMinAsync(id);
        return ToQueryResult(result);
    }



    public async Task<ServiceResult<Guid>> CreateAsync(CreateAssignmentDto dto, Guid client)
    {
        var user = await _uow.UserRepo.Find(client);
        if (user is null)
            return Unauthorized<Guid>();

        var entity = Adapt(dto, user);
        await _uow.AssignmentRepo.Insert(entity);
        return Created(entity.Id);
    }

    public Task<ServiceResult> UpdateAsync(UpdateAssignmentDto dto, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(Guid id, Guid client)
    {
        throw new NotImplementedException();
    }






    private Assignment Adapt(CreateAssignmentDto dto, User creator)
    {
        Guid assignmentId = Guid.NewGuid();

        var questions = dto.Questions.Select(_ =>
            new McqQuestion(
                _.Content,
                assignmentId,
                _.Choices.Select(_ => new McqChoice(_.Content, _.IsCorrect)).ToList()
            )
        ).ToList();
        return new Assignment(assignmentId, dto.Name, dto.Duration, dto.SectionId, creator, questions);
    }
}
