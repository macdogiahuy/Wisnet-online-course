using CourseHub.Core.Entities.CourseDomain.Enums;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.Core.RequestDtos.Course.LectureDtos;
using CourseHub.UI.Helpers;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Helpers.Utils;
using CourseHub.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseHub.UI.Pages.Course;

public class UpdateModel : PageModel
{
    // UpdateCourse
    [BindProperty]
    public UpdateCourseDto UpdateCourseDto { get; set; }

    // CreateLecture
    [BindProperty]
    public CreateLectureDto CreateLectureDto { get; set; }
    [BindProperty]
    public IFormFile[] Files { get; set; }



    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var client = context.HttpContext.GetClientData().Result;
        if (client is null || client.Role < Core.Entities.UserDomain.Enums.Role.Instructor)
            context.Result = Redirect(Global.PAGE_404);
    }

    public IActionResult OnGet([FromQuery] Guid sectionId)
    {
        if (sectionId == default)
            return Redirect(Global.PAGE_404);

        CreateLectureDto = new CreateLectureDto { SectionId = sectionId };
        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
	}

    public async Task<IActionResult> OnPostCreateLecture([FromServices] ILectureApiService lectureApiService)
    {
        CreateLectureDto.Materials = new();
        for (int i = 0; i < Files.Length; i++)
            CreateLectureDto.Materials.Add(new CreateLectureDto.CreateLectureMaterialDto
            {
                Type = ResourceHelper.IsVideo(Files[i]) ? LectureMaterialType.Video : LectureMaterialType.Document,
                File = Files[i]
            });

        var response = await lectureApiService.Create(CreateLectureDto, HttpContext);

        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_MESSAGE] = "Cannot create lecture!";
            TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
        }

        Guid sectionId = CreateLectureDto.SectionId;
        CreateLectureDto = new();
        return OnGet(sectionId);
    }
}
