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

    public async Task<ServiceResult<Guid>> CreateAsync(CreateSubmissionDto dto, Guid client)
    {
        try
        {
            //...
            var choices = await _uow.McqChoiceRepo.GetMultiple(dto.Answers.Select(_ => _.MCQChoiceId));

            var entity = Adapt(dto, client, choices);
            await _uow.SubmissionRepo.Insert(entity);
            return Created(entity.Id);
        }
        catch
        {
            return ServerError<Guid>();
        }
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





    
    private Submission Adapt(CreateSubmissionDto _, Guid client, List<McqChoice> choices)
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
            Mark = correctChoices / choices.Count
        };
    }
}
