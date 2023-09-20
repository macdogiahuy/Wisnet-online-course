using CourseHub.API.Controllers.Shared;
using CourseHub.Core.RequestDtos.Common.NotificationDtos;
using CourseHub.Core.RequestDtos.Course.InstructorDtos;
using CourseHub.Core.Services.Domain.CommonServices;
using CourseHub.Core.Services.Domain.CourseServices;
using CourseHub.Core.Entities.CommonDomain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using CourseHub.Core.Entities.UserDomain.Enums;
using CourseHub.API.Helpers.Cookie;

namespace CourseHub.API.Controllers.CourseControllers;

[Produces(contentType: MediaTypeNames.Application.Json)]
public class InstructorsController : BaseController
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }



    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Get(QueryInstructorDto dto)
    {
        var result = await _instructorService.GetAsync(dto);
        return result.AsResponse();
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Learner))]
    public async Task<IActionResult> RequestInstructorRole([FromServices] INotificationService notificationService)
    {
        CreateNotificationDto dto = new() { Type = NotificationType.RequestToBecomeInstructor };
        var client = HttpContext.GetClientId();
        var result = await notificationService.CreateAsync(dto, client);
        return result.AsResponse();
    }

    [HttpPatch]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Update(UpdateInstructorDto dto)
    {
        var result = await _instructorService.UpdateAsync(dto);
        return result.AsResponse();
    }
}
