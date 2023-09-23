using CourseHub.UI.Helpers.AppStart;
using System.Text.RegularExpressions;

namespace CourseHub.UI.Helpers.Utils;

public static class TextHelper
{
    private const string PATTERN_GUID = @"\b[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\b";

    public static string ToInputDate(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    public static string? GetCourseResourcePath(string resourceUrl)
    {
        List<Guid> guids = GetGuidsFromString(resourceUrl, 2);
        if (guids.Count == 2)
            return Configurer.GetApiClientOptions().ApiServerPath + $"/api/courses/resource/{guids[0]}/{guids[1]}";
        return null;
    }






    public static List<Guid> GetGuidsFromString(string input, int count)
    {
        List<Guid> guids = new();

        MatchCollection matches = Regex.Matches(input, PATTERN_GUID);

        foreach (Match match in matches.Cast<Match>())
        {
            if (Guid.TryParse(match.Value, out Guid result))
            {
                guids.Add(result);
                if (guids.Count == count)
                    break;
            }
        }
        return guids;
    }

    public static Guid? GetFirstGuidFromString(string input)
    {
        Match match = Regex.Match(input, PATTERN_GUID);

        if (match.Success && Guid.TryParse(match.Value, out Guid result))
            return result;
        return null;
    }
}
