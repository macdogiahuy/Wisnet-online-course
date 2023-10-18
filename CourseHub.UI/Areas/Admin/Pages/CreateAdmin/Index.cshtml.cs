using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts.UserServices;
using CourseHub.UI.Services.Implementations.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Areas.Admin.Pages.CreateAdmin;

public class IndexModel : PageModel
{
    [BindProperty]
    public CreateUserDto Dto { get; set; }
    [BindProperty]
    public string RePassword { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost([FromServices] IUserApiService userApiService)
    {
        if (!ModelState.IsValid)
            return Redirect(Request.Path);
        if (RePassword != Dto.Password)
        {
            ModelState.AddModelError("RePassword", "Password and RePassword doesn't match");
            return Redirect(Request.Path);
        }

        var response = await userApiService.CreateAdminAsync(Dto, HttpContext);

        /*TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_MESSAGE] = "Cannot register!";
            return;
        }*/

        return Redirect(Request.Path);
    }
}
