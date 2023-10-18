using CourseHub.Core.RequestDtos.Payment;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.CommonServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Instructor;

public class WithdrawModel : PageModel
{
    [BindProperty]
    public CreateWithdrawalDto Dto { get; set; }



    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var client = context.HttpContext.GetClientData().Result;
        if (client is null || client.Role != Core.Entities.UserDomain.Enums.Role.Instructor)
            context.Result = Redirect(Global.PAGE_SIGNIN);
    }



    public void OnGet()
    {
        TempData[Global.DATA_USE_BACKGROUND] = true;
    }

    public async Task<IActionResult> OnPost([FromServices] INotificationApiService notificationApiService)
    {
        if (!ModelState.IsValid)
        {
            TempData[Global.ALERT_STATUS] = false;
            TempData[Global.ALERT_MESSAGE] = "Invalid data!";
            TempData[Global.DATA_USE_BACKGROUND] = true;
            return Page();
        }

        var response = await notificationApiService.CreateWithdrawRequest(Dto, HttpContext);
        if (response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
            TempData[Global.ALERT_MESSAGE] = response.IsSuccessStatusCode ?
                "Requested successfully." :
                $"Cannot request for withdrawal!";
        }
        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
    }
}
