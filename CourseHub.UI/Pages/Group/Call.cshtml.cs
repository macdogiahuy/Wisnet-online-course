using CourseHub.Core.Models.User.UserModels;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Group;

public class CallModel : PageModel
{
    public Guid ConversationId { get; set; }

    public UserFullModel? Client { get; set; }

    public async Task<IActionResult> OnGet(Guid id)
    {
        Client = await HttpContext.GetClientData();
        if (Client is null)
            return Redirect(Global.PAGE_SIGNIN);

        ConversationId = id;
        return Page();
    }
}
