using CourseHub.Core.Models.Assignment.AssignmentModels;
using CourseHub.Core.RequestDtos.Assignment.AssignmentDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Services.Contracts.AssignmentServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;

public class UpdateModel : PageModel
{
    private readonly IAssignmentApiService _assignmentApiService;

    public AssignmentModel Assignment { get; set; }
    public string DeleteSectionPath { get; set; }

    [BindProperty]
    public UpdateAssignmentDto Dto { get; set; }
    [BindProperty]
    public IFormFile File { get; set; }



    public UpdateModel(IAssignmentApiService assignmentApiService)
    {
        _assignmentApiService = assignmentApiService;
    }


    public async Task<IActionResult> OnGet([FromQuery] Guid id)
    {
        if (id == default)
            return Redirect(Global.PAGE_404);

        var assignment = await _assignmentApiService.GetAsync(id, HttpContext);
        if (assignment is null)
            return Redirect(Global.PAGE_404);

        Assignment = assignment;
        TempData[Global.DATA_USE_BACKGROUND] = true;
        DeleteSectionPath = Configurer.GetApiClientOptions().ApiServerPath + "/api/assignments";
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        //...
        TempData[Global.ALERT_STATUS] = true;
        TempData[Global.ALERT_MESSAGE] = "Updated Assignment!";
        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Redirect(Request.Path + "?id=" + Dto.Id);
    }
}
