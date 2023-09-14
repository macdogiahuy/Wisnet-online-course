using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.User;

public class SignOutModel : PageModel
{
    public IActionResult OnGet() => HandleSignOut();

    public IActionResult OnPost() => HandleSignOut();

    private IActionResult HandleSignOut()
    {
        Response.ExpireAuthCookies();
        HttpContext.Session.Clear();
        return RedirectToPage(Global.PAGE_INDEX);
    }
}
