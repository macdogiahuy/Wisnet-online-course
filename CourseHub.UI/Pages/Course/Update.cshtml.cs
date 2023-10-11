using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.CourseServices;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Course;

public class UpdateModel : PageModel
{
    private readonly ICourseApiService _courseApiService;


    // In each forms without CourseId
    [BindProperty]
    public Guid CourseId { get; set; }

    public CourseModel? Course { get; set; }

    public List<Category> Categories { get; set; }



    // UpdateCourse
    [BindProperty]
    public UpdateCourseDto UpdateCourseDto { get; set; }

    // Delete Section
    public string DeleteSectionPath { get; set; }



    public UpdateModel(ICourseApiService courseApiService)
    {
        _courseApiService = courseApiService;
    }



    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var client = context.HttpContext.GetClientData().Result;
        if (client is null || client.Role < Core.Entities.UserDomain.Enums.Role.Instructor)
            context.Result = Redirect(Global.PAGE_404);

        DeleteSectionPath = Configurer.GetApiClientOptions().ApiServerPath + "/api/courses";
    }

    public async Task<IActionResult> OnGet(
        [FromQuery] Guid courseId,
        [FromServices] ICategoryApiService categoryApiService)
    {
        if (courseId == default)
            return Redirect(Global.PAGE_404);
        Course = await _courseApiService.GetAsync(courseId);
        if (Course is null)
            return Redirect(Global.PAGE_404);

        Categories = await categoryApiService.GetAsync();

        if (courseId != default)
        {
            UpdateCourseDto = new UpdateCourseDto { Id = courseId };
        }

        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
	}

    public async Task<IActionResult> OnPostUpdateCourse()
    {
        var response = await _courseApiService.UpdateAsync(UpdateCourseDto, HttpContext);

        TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_MESSAGE] = "Cannot update course!";
            return Redirect(Request.Path + $"?courseId={UpdateCourseDto.Id}");
        }
        TempData[Global.ALERT_MESSAGE] = "Update course successfully!";

        return Redirect(Request.Path + $"?courseId={UpdateCourseDto.Id}");
    }
}
