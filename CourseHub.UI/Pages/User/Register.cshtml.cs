using CourseHub.Core.Helpers.Validation;
using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.User;

public class RegisterModel : PageModel
{
    [BindProperty]
    public CreateUserDto Dto { get; set; }
    [BindProperty]
    public string RePassword { get; set; }


    public async Task<IActionResult> OnPost([FromServices] IUserApiService userApiService)
    {
        if (!ModelState.IsValid)
            return Page();
        if (RePassword != Dto.Password)
        {
            ModelState.AddModelError("RePassword", "Password and RePassword doesn't match");
            return Page();
        }

        var response = await userApiService.RegisterAsync(Dto);

        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_MESSAGE] = "Cannot register!";
            TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
            return Page();
        }
        return Redirect(Global.PAGE_SIGNIN);
    }
}
