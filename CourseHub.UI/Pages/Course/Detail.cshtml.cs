using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.Course.InstructorModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Course;

public class DetailModel : PageModel
{
    private readonly ICourseApiService _courseApiService;

    public CourseModel? Course { get; set; }
    public InstructorModel? Instructor { get; set; } = new InstructorModel { };
    public UserModel? InstructorUser { get; set; } = new UserModel { };

    public List<CourseOverviewModel> MoreCourses { get; set; } = new();

    public DetailModel(ICourseApiService courseApiService)
    {
        _courseApiService = courseApiService;
    }

    public async Task<IActionResult> OnGet(
        [FromQuery] Guid id,
        [FromServices] IUserApiService userApiService)
    {
        if (id == default)
            return Redirect(Global.PAGE_404);

        Course = await _courseApiService.GetAsync(id);
        if (Course is null)
            return Redirect(Global.PAGE_404);

        //...
        Course.Reviews = new();
        InstructorUser = await userApiService.GetAsync(Course.Creator.Id);

        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
