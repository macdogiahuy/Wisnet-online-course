using AutoMapper;
using CourseHub.Core.Entities.AssignmentDomain;
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



    public async Task<ServiceResult<SubmissionModel>> GetAsync(Guid id)
    {
        var result = await _uow.SubmissionRepo.Get(id);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<List<SubmissionMinModel>>> GetByAssignmentId(Guid assignmentId)
    {
        var result = await _uow.SubmissionRepo.GetByAssignmentId(assignmentId);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<Guid>> CreateAsync(CreateSubmissionDto dto, Guid client)
    {
        try
        {
            // A lot of queries
            //...
            var assignment = await _uow.AssignmentRepo.Find(dto.AssignmentId);
            if (assignment is null)
                return NotFound<Guid>();
            var choices = await _uow.McqChoiceRepo.GetMultiple(dto.Answers.Select(_ => _.MCQChoiceId));

            var entity = Adapt(dto, client, choices, assignment);
            await _uow.SubmissionRepo.Insert(entity);
            await _uow.CommitAsync();

            /*var section = await _uow.SectionRepo.GetWithCourse(assignment.SectionId);
            var passed = await _uow.SubmissionRepo.Get
            var course = await _uow.CourseRepo.GetAsync
            section.Course.Id*/

            return Created(entity.Id);
        }
        catch
        {
            return ServerError<Guid>();
        }
    }

    public Task<ServiceResult> UpdateAsync(UpdateSubmissionDto dto, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(Guid id, Guid client)
    {
        throw new NotImplementedException();
    }






    private Submission Adapt(CreateSubmissionDto _, Guid client, List<McqChoice> choices, Assignment assignment)
    {
        Guid id = Guid.NewGuid();

        List<McqUserAnswer> answers = choices.Select(_ => new McqUserAnswer
        {
            SubmissionId = id,
            MCQChoiceId = _.Id
        }).ToList();

        int correctChoices = 0;
        foreach (var choice in choices)
            if (choice.IsCorrect)
                correctChoices++;

        return new Submission
        {
            Id = id,
            CreatorId = client,
            AssignmentId = _.AssignmentId,
            TimeSpentInSec = _.TimeSpentInSec,
            Answers = answers,
            Mark = (correctChoices / (double)assignment.QuestionCount) * 10
        };
    }
}
