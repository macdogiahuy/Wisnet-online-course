using CourseHub.Core.Models.Assignment.AssignmentModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Assignment.SubmissionDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.AssignmentServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;

public class AttemptModel : PageModel
{
    public AssignmentModel Assignment { get; set; }
    public UserFullModel Client { get; set; }

    [BindProperty]
    public CreateSubmissionDto Dto { get; set; }
    [BindProperty]
    public Guid[] AnswerChoices { get; set; }



    public async Task<IActionResult> OnGet(
        [FromQuery] Guid id,
        [FromServices] IAssignmentApiService assignmentApiService)
    {
#pragma warning disable CS8601
        Assignment = await assignmentApiService.GetAsync(id, HttpContext);
        Client = await HttpContext.GetClientData();
#pragma warning restore CS8601



        if (Client is null)
            return Redirect(Global.PAGE_SIGNIN);
        if (Assignment is null)
            return Redirect(Global.PAGE_404);



        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }

    public async Task<IActionResult> OnPost([FromServices] ISubmissionApiService submissionApiService)
    {
        Dto.Answers = AnswerChoices
            .Select(_ => new CreateSubmissionDto.CreateMcqUserAnswerDto { MCQChoiceId = _ })
            .ToList();

        /*if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
                System.Diagnostics.Debug.WriteLine(state.Value.Errors);
            TempData[Global.DATA_USE_BACKGROUND] = true;
            //...
            return Redirect(Request.Path);
        }*/

        var response = await submissionApiService.CreateAsync(Dto, HttpContext);
        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.DATA_USE_BACKGROUND] = true;
            //...
            return Redirect(Request.Path + $"?id={Dto.AssignmentId}");
        }

        return Redirect($"{Global.PAGE_ASSIGNMENT_OVERVIEW}?id={Dto.AssignmentId}");
    }
}
