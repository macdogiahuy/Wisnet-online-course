using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Utils;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Lecture;

public class DetailModel : PageModel
{
    public Core.Entities.CourseDomain.Lecture Lecture { get; set; }
    public CourseOverviewModel Course { get; set; }




    public async Task<IActionResult> OnGet([FromQuery] Guid id,
        [FromServices] ILectureApiService lectureApiService, [FromServices] ICourseApiService courseApiService, [FromServices] IUserApiService userApiService)
    {
        TempData[Global.DATA_USE_BACKGROUND] = true;

        Lecture = await lectureApiService.GetAsync(id, HttpContext);
        if (Lecture is null)
            return Redirect(Global.PAGE_404);

        Course = await courseApiService.GetBySectionIdAsync(Lecture.SectionId);
        if (Course is null)
            return Redirect(Global.PAGE_404);

        Course.Creator.AvatarUrl = userApiService.GetAvatarApiUrl(Course.Creator.AvatarUrl, Course.Id);

        return Page();
    }
}
