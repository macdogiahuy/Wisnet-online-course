using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.User;

public class IndexModel : PageModel
{
    public void OnGet([FromRoute] Guid id)
    {

    }
}
