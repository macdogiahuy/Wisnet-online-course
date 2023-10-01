using CourseHub.Core.Models.User.UserModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Social;

public class IndexModel : PageModel
{
    public UserFullModel Client { get; set; }
    public List<UserOverviewModel> RelatedUsers { get; set; }

    public async Task<IActionResult> OnGet([FromServices] IUserApiService userApiService)
    {
        Client = await HttpContext.GetClientData();
        if (Client is null)
            return Redirect(Global.PAGE_SIGNIN);



        List<Guid> relatedUsers = new();
        RelatedUsers = await userApiService.GetOverviewAsync(relatedUsers);
        return Page();
    }
}
