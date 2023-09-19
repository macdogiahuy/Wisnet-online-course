using System.Text.Json;

namespace CourseHub.UI.Helpers.Http;

public static class SerializeOptions
{
    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
}
