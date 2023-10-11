using CourseHub.Core.Entities.AssignmentDomain;
using CourseHub.Core.Models.Assignment.AssignmentModels;
using CourseHub.Core.Models.Assignment.McqQuestionModels;
using CourseHub.Core.RequestDtos.Assignment.SubmissionDtos;
using CourseHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;

public class AttemptModel : PageModel
{
    public AssignmentModel Assignment { get; set; }

    [BindProperty]
    public CreateSubmissionDto Dto { get; set; }



    public void OnGet([FromRoute] Guid id)
    {
        Assignment = new AssignmentModel
        {
            Id = id,
            Name = "Assignment",
            Duration = 30,
            QuestionCount = 20,
            Section = new Core.Models.Course.SectionModels.SectionMinModel { Id = Guid.NewGuid(), Title = "" },
            Questions = new()
        };

        for (int i = 0; i < 20; i++)
        {
            Assignment.Questions.Add(new McqQuestionModel
            {
                Id = Guid.NewGuid(),
                Content = "An employee at the local ice cream parlor asks three customers if they like chocolate ice cream. What is the population?",
                Choices = new()
                {
                    new McqChoice { Id = Guid.NewGuid(), Content = "all custormers" },
                    new McqChoice { Id = Guid.NewGuid(), Content = "three selected custermers" },
                    new McqChoice { Id = Guid.NewGuid(), Content = "all men custormers" },
                    new McqChoice { Id = Guid.NewGuid(), Content = "all women custormers" },
                }
            });
        }

        TempData[Global.DATA_USE_BACKGROUND] = true;
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Unauthorized();
    }
}
