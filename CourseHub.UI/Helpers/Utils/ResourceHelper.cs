namespace CourseHub.UI.Helpers.Utils;

public static class ResourceHelper
{
    public static bool IsRemote(string resourceUrl)
    {
        return resourceUrl.StartsWith("http");
    }
}
