using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Entities.CourseDomain.Enums;
using CourseHub.Core.Models.Course.CourseModels;
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
    private readonly ICourseApiService _courseApiService;

    // In each forms without CourseId
    [BindProperty]
    public Guid CourseId { get; set; }

    public CourseModel? Course { get; set; }



    // UpdateCourse
    [BindProperty]
    public UpdateCourseDto UpdateCourseDto { get; set; }

    // CreateLecture
    [BindProperty]
    public CreateLectureDto CreateLectureDto { get; set; }
    [BindProperty]
    public IFormFile[] Files { get; set; }

    public List<Category> Categories { get; set; }




    public UpdateModel(ICourseApiService courseApiService)
    {
        _courseApiService = courseApiService;
    }



    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var client = context.HttpContext.GetClientData().Result;
        if (client is null || client.Role < Core.Entities.UserDomain.Enums.Role.Instructor)
            context.Result = Redirect(Global.PAGE_404);
    }

    public async Task<IActionResult> OnGet([FromQuery] Guid courseId, [FromQuery] Guid? sectionId = null)
    {
        if (courseId == default)
            return Redirect(Global.PAGE_404);
        Course = await _courseApiService.GetAsync(courseId);
        if (Course is null)
            return Redirect(Global.PAGE_404);



        if (courseId != default)
        {
            UpdateCourseDto = new UpdateCourseDto { Id = courseId };
        }
        else if (sectionId is not null && sectionId != default)
        {
            CreateLectureDto = new CreateLectureDto { SectionId = (Guid)sectionId };
        }



        TempData[Global.DATA_USE_BACKGROUND] = true;
        return Page();
	}

    public async Task<IActionResult> OnPostUpdateCourse()
    {
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

        TempData[Global.ALERT_STATUS] = response.IsSuccessStatusCode;
        if (!response.IsSuccessStatusCode)
        {
            TempData[Global.ALERT_MESSAGE] = "Cannot create lecture!";
        }

        Guid sectionId = CreateLectureDto.SectionId;
        CreateLectureDto = new();
        return await OnGet(CourseId, sectionId);
    }
}
