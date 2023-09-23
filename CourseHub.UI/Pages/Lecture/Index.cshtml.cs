using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Lecture;

public class IndexModel : PageModel
{
    public Core.Entities.CourseDomain.Lecture Lecture { get; set; }

    public async Task<IActionResult> OnGet([FromQuery] Guid id, [FromServices] ILectureApiService lectureApiService)
    {
        TempData[Global.DATA_USE_BACKGROUND] = true;
        var result = await lectureApiService.GetAsync(id, HttpContext);
        if (result is null)
            return Redirect(Global.PAGE_404);

        Lecture = result;
        return Page();
    }
}
