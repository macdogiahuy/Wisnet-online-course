using CourseHub.Core.RequestDtos.Assignment.McqQuestionDtos;

namespace CourseHub.Core.RequestDtos.Assignment.AssignmentDtos;

public class CreateAssignmentDto
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public Guid SectionId { get; set; }
    public List<CreateMcqQuestionDto> Questions { get; set; }
}
