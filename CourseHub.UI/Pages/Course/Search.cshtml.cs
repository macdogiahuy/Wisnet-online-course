using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Course;

public class SearchModel : PageModel
{
    private readonly ICourseApiService _courseApiService;

    public PagedResult<CourseOverviewModel> Courses { get; set; }

    public SearchModel(ICourseApiService courseApiService)
    {
        _courseApiService = courseApiService;
    }

    public async Task OnGet()
    {
        TempData[Global.DATA_USE_BACKGROUND] = true;
        QueryCourseDto dto = new();
        Courses = await _courseApiService.GetPagedAsync(dto);
    }
}
