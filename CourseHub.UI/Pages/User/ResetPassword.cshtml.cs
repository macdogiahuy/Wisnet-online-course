using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.User;

public class ResetPasswordModel : PageModel
{
    [BindProperty]
    public ResetPasswordDto ResetPasswordDto { get; set; }

    [BindProperty]
    public string RePassword { get; set; }

    public string Email { get; set; }
    public string Token { get; set; }

    public void OnGet([FromRoute] string email, [FromRoute] string token)
    {
        Email = email;
        Token = token;
    }

    public async Task<IActionResult> OnPost(
        [FromRoute] string email, [FromRoute] string token,
        [FromServices] IUserApiService userApiService)
    {
        if (RePassword != ResetPasswordDto.NewPassword)
            ModelState.AddModelError(nameof(RePassword), "Passwords do not match");
        if (!ModelState.IsValid)
            return ReDisplay(email, token);

        

        var response = await userApiService.ResetPasswordAsync(ResetPasswordDto);

        if (response.IsSuccessStatusCode)
            return Redirect(Global.PAGE_SIGNIN);

        TempData[Global.ALERT_MESSAGE] = "Invalid token!";
        TempData[Global.ALERT_STATUS] = false;
        return ReDisplay(email, token);
    }

    private IActionResult ReDisplay(string email, string token)
    {
        Email = email;
        Token = token;
        return Page();
    }
}
