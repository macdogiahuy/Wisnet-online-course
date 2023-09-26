using CourseHub.UI.Helpers.AppStart;

namespace CourseHub.UI.Helpers.Utils;

public static class TextHelper
{
    public static string ToInputDate(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    public static string? GetCourseResourcePath(string resourceUrl)
    {
        List<Guid> guids = Core.Helpers.Text.TextHelper.GetGuidsFromString(resourceUrl, 2);
        if (guids.Count == 2)
            return Configurer.GetApiClientOptions().ApiServerPath + $"/api/courses/resource/{guids[0]}/{guids[1]}";
        return null;
    }
}
