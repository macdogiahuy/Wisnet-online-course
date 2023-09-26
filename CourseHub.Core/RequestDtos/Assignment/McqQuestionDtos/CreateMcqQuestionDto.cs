namespace CourseHub.Core.RequestDtos.Assignment.McqQuestionDtos;

public class CreateMcqQuestionDto
{
    public string Content { get; set; }
    public List<CreateMcqChoiceDto> Choices { get; set; }

    public class CreateMcqChoiceDto
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }
}
