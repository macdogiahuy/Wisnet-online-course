using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using CourseHub.Core.Interfaces.Authentication;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.API.Helpers.Cookie;
using CourseHub.API.Controllers.Shared;
using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.Core.Services.Domain.UserServices.TempModels;
using CourseHub.Core.Services.Domain.UserServices;

namespace CourseHub.API.Controllers.UserControllers;

[Consumes(contentType: MediaTypeNames.Application.Json)]
[Produces(contentType: MediaTypeNames.Application.Json)]
public class AuthController : BaseController
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }






    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto, [FromServices] ITokenService tokenService)
    {
        ServiceResult<AuthDto> result = await _userService.SignInAsync(dto, tokenService);

        if (result.IsSuccessful)
            SetAuthData(result.Data!);
        return result.AsResponse();
    }

    [HttpPost("SignOut")]
    public new void SignOut()
    {
        Response.SetCredentials("", "", CookieHelper.GetExpiredOptions());
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromServices] ITokenService tokenService)
    {
        ServiceResult<AuthDto> result = await _userService.RefreshAsync(
            Request.GetAccessToken(),
            Request.GetRefreshToken(),
            tokenService
        );

        if (result.IsSuccessful)
            SetAuthData(result.Data!);
        return result.AsResponse();
    }



    private void SetAuthData(AuthDto authData)
    {
        Response.SetCredentials(authData.AccessToken, authData.RefreshToken, CookieHelper.GetOptions());
    }
}
