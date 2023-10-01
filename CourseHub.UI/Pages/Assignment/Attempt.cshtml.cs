using CourseHub.Core.Models.Assignment.McqQuestionModels;
using CourseHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;
public class AttemptModel : PageModel
{
    public List<McqQuestionModel> Questions { get; set; }

    public void OnGet()
    {
        Questions = new List<McqQuestionModel>();
        for (int i = 0; i < 20; i++)
            Questions.Add(new McqQuestionModel());

        TempData[Global.DATA_USE_BACKGROUND] = true;
    }
}
