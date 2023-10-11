using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Helpers.Utils;
using CourseHub.UI.Services.Contracts.CourseServices;
using CourseHub.UI.Services.Contracts.UserServices;
using CourseHub.UI.Services.Implementations.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Lecture;

public class DetailModel : PageModel
{
    public Core.Entities.CourseDomain.Lecture Lecture { get; set; }
    public UserFullModel Client { get; set; }
    public CourseOverviewModel Course { get; set; }




    public async Task<IActionResult> OnGet([FromQuery] Guid id,
        [FromServices] ILectureApiService lectureApiService, [FromServices] ICourseApiService courseApiService, [FromServices] IUserApiService userApiService)
    {
        Lecture = await lectureApiService.GetAsync(id, HttpContext);
        if (Lecture is null)
            return Redirect(Global.PAGE_404);

        Client = await HttpContext.GetClientData();
        if (!Lecture.IsPreviewable && Client is null)
            return Unauthorized();

        Course = await courseApiService.GetBySectionIdAsync(Lecture.SectionId);
        if (Course is null)
            return Redirect(Global.PAGE_404);

        var isEnrolled = await courseApiService.IsEnrolled(Course.Id, HttpContext);
        if (!Lecture.IsPreviewable && !isEnrolled)
            return Unauthorized();

        Course.Creator.AvatarUrl = UserApiService.GetAvatarApiUrl(Course.Creator.AvatarUrl, Course.Id);

        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
