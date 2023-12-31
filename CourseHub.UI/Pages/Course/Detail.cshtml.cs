using CourseHub.Core.Entities.CommonDomain;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Common.CommentModels;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.Course.CourseReviewModels;
using CourseHub.Core.Models.Course.InstructorModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Course.CourseReviewDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.CommonServices;
using CourseHub.UI.Services.Contracts.CourseServices;
using CourseHub.UI.Services.Contracts.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Course;

public class DetailModel : PageModel
{
    private readonly ICourseApiService _courseApiService;



    public UserFullModel? Client { get; set; }
    public CourseModel? Course { get; set; }
    public InstructorModel? Instructor { get; set; } = new InstructorModel { };
    public UserModel? InstructorUser { get; set; } = new UserModel { };
    public bool IsEnrolled { get; set; }



    public PagedResult<CourseReviewModel> Reviews { get; set; }
    public List<CourseOverviewModel> MoreCourses { get; set; } = new();
    public List<UserMinModel> RelatedUsers { get; set; }



    // Common for forms
    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateCourseReviewDto CreateCourseReviewDto { get; set; }






    public DetailModel(ICourseApiService courseApiService)
    {
        _courseApiService = courseApiService;
    }

    public async Task<IActionResult> OnGet(
        [FromQuery] Guid id,
        [FromServices] IUserApiService userApiService, [FromServices] ICourseReviewApiService reviewApiService)
    {
        return await RenderPage(id, userApiService, reviewApiService);
    }

	public async Task<IActionResult> OnPostCreateReview(
        [FromServices] ICourseReviewApiService reviewApiService)
	{
        await reviewApiService.CreateAsync(CreateCourseReviewDto, HttpContext);

        return Redirect(Request.Path + $"?id={Id}");
    }






    private async Task<IActionResult> RenderPage(
        Guid id,
        IUserApiService userApiService, ICourseReviewApiService reviewApiService)
    {
        if (id == default)
            return Redirect(Global.PAGE_404);

        var courseTask = _courseApiService.GetAsync(id);
        var clientTask = HttpContext.GetClientData();
        await Task.WhenAll(courseTask, clientTask);
        Course = courseTask.Result;
        Client = clientTask.Result;
        if (Course is null)
            return Redirect(Global.PAGE_404);

        //...
        Course.Reviews = new();
        var instructorUserTask = userApiService.GetAsync(Course.Creator.Id);
        var isEnrolledTask = _courseApiService.IsEnrolled(Course.Id, HttpContext);
        var reviewsTask = reviewApiService.GetAsync(new QueryCourseReviewDto { CourseId = Course.Id });
        var moreCoursesTask = _courseApiService.GetPagedAsync(
            new Core.RequestDtos.Course.CourseDtos.QueryCourseDto
            {
                PageSize = 4,
                InstructorId = Course.InstructorId
            });

        await Task.WhenAll(instructorUserTask, isEnrolledTask, reviewsTask, moreCoursesTask);
        InstructorUser = instructorUserTask.Result;
        IsEnrolled = isEnrolledTask.Result;
        Reviews = reviewsTask.Result;
        MoreCourses = moreCoursesTask.Result.Items.Where(_ => _.Id != Course.Id).ToList();

        if (Reviews.TotalCount > 0)
        {
            RelatedUsers = await userApiService.GetMinAsync(Reviews.Items.Select(_ => _.CreatorId).Distinct());
        }

        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
