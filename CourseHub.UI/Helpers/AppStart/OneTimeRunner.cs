namespace CourseHub.UI.Helpers.AppStart;

public static class OneTimeRunner
{
    private static bool _initedConfig;

    public static void InitConfig(WebApplicationBuilder builder)
    {
        if (_initedConfig)
            return;

        Configurer.Init(builder.Configuration);

        _initedConfig = true;
    }

    public static void ColdRequest()
    {
        //...
        new HttpClient().GetAsync("https://localhost:7277/api/courses/Resource/Media/CourseMedia/69746c85-6109-4370-9334-1490cd2334b0/ce7c409d-a615-4336-b430-8c5bd17d927c.mp4");
    }
}
