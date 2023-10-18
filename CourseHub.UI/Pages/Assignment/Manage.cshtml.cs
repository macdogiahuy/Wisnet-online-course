using CourseHub.Core.Models.Assignment.AssignmentModels;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts.AssignmentServices;
using CourseHub.UI.Services.Contracts.CourseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;

public class ManageModel : PageModel
{
    private readonly IAssignmentApiService _assignmentApiService;

    public List<AssignmentMinModel> Assignments { get; set; }
    //public CourseModel Course { get; set; }



    public ManageModel(IAssignmentApiService assignmentApiService)
    {
        _assignmentApiService = assignmentApiService;
    }

    public async Task<IActionResult> OnGet([FromQuery] Guid courseId, [FromServices] ICourseApiService courseApiService)
    {
        if (courseId == default)
            return Redirect(Global.PAGE_404);

        var courseTask = courseApiService.GetAsync(courseId);
        var assignmentTask = _assignmentApiService.GetByCourseAsync(courseId);
        await Task.WhenAll(courseTask, assignmentTask);
        //Course = courseTask.Result;
        Assignments = assignmentTask.Result;
        /*if (Course is null)
            return Redirect(Global.PAGE_404);*/



        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
