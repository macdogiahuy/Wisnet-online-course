using CourseHub.API.Controllers.Shared;
using CourseHub.API.Helpers.Cookie;
using CourseHub.Core.RequestDtos.Social.ConversationDtos;
using CourseHub.Core.Services.Domain.SocialServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers.SocialControllers;

public class ConversationsController : BaseController
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }



    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetConversations([FromQuery] QueryConversationDto dto)
    {
        var client = HttpContext.GetClientId();
        var result = await _conversationService.Get(dto, (Guid)client!);
        return result.AsResponse();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateConversation(CreateConversationDto dto)
    {
        var client = HttpContext.GetClientId();
        var result = await _conversationService.Create(dto, (Guid)client!);
        return result.AsResponse();
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> UpdateConversations([FromForm] UpdateConversationDto dto)
    {
        var client = HttpContext.GetClientId();
        var result = await _conversationService.Update(dto, (Guid)client!);
        return result.AsResponse();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var client = HttpContext.GetClientId();
        var result = await _conversationService.Delete(id, (Guid)client!);
        return result.AsResponse();
    }
}
