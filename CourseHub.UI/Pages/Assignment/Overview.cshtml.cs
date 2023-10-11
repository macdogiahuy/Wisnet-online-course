using CourseHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Assignment;

public class ReviewModel : PageModel
{


    public void OnGet([FromRoute] Guid id)
    {


        TempData[Global.DATA_USE_BACKGROUND] = true;
    }
}
