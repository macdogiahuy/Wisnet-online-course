using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Assignment.AssignmentDtos;
using CourseHub.Core.RequestDtos.Assignment.McqQuestionDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.AssignmentServices;
using CourseHub.UI.Services.Contracts.CourseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;

namespace CourseHub.UI.Pages.Assignment;

public class CreateModel : PageModel
{
    public Guid SectionId { get; set; }
    public CourseOverviewModel Course { get; set; }

    [BindProperty]
    public CreateAssignmentDto Dto { get; set; }
    [BindProperty]
    public IFormFile File { get; set; }



    /*public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var client = context.HttpContext.GetClientData().Result;
        if (client is null || client.Role != Core.Entities.UserDomain.Enums.Role.Instructor)
            context.Result = Redirect(Global.PAGE_404);
    }*/



    public async Task<IActionResult> OnGet([FromQuery] Guid sectionId, [FromServices] ICourseApiService courseApiService)
    {
        if (sectionId == default)
            return Redirect(Global.PAGE_404);

        var course = await courseApiService.GetBySectionIdAsync(sectionId);
        if (course is null)
            return Redirect(Global.PAGE_404);

        SectionId = sectionId;
        Course = course;
        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }

    public async Task<IActionResult> OnPost([FromServices] IAssignmentApiService assignmentApiService)
    {
        //...
        if (File is null || File.ContentType != "application/json")
        {
            TempData[Global.ALERT_STATUS] = false;
            TempData[Global.ALERT_MESSAGE] = "The file format is invalid";
            TempData[Global.DATA_USE_BACKGROUND] = true;
            return Page();
        }

        using var reader = new StreamReader(File.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        List<CreateMcqQuestionDto>? questions = JsonSerializer.Deserialize<List<CreateMcqQuestionDto>>(content);

        //...
        if (questions is null)
        {
            TempData[Global.ALERT_STATUS] = false;
            TempData[Global.ALERT_MESSAGE] = "The file format is invalid";
            TempData[Global.DATA_USE_BACKGROUND] = true;
            return Page();
        }

        Dto.Questions = questions;
        var response = await assignmentApiService.CreateAsync(Dto, HttpContext);
        TempData[Global.ALERT_STATUS] = true;
        TempData[Global.ALERT_MESSAGE] = "Created Assignment!";
        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
