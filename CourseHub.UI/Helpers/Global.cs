using CourseHub.UI.Helpers.AppStart;

namespace CourseHub.UI.Helpers;

public class Global
{
    public const string APP_NAME = "WisNet";

    // common objects' names
    public const string TITLE = "Title";
    public const string ALERT_MESSAGE = "AlertMessage";
    public const string ALERT_STATUS = "AlertStatus";
    public const string DATA_IGNORE_NAV = "IgnoreNav";
    public const string DATA_USE_BACKGROUND = "UseBackground";

    // anchor's asp-page
    public const string PAGE_INDEX = "/Index";
    public const string PAGE_404 = "/Shared/404";
    public const string PAGE_ADMIN = "/Admin";

    public const string PAGE_SIGNIN = "/SignIn";
    public const string PAGE_REGISTER = "/Register";
    public const string PAGE_PROFILE = "/Profile";
    public const string PAGE_SIGNOUT = "/User/SignOut";
    public const string PAGE_FORGOT_PASSWORD = "/forgot-password";
    public const string PAGE_CHANGE_PASSWORD = "/User/ChangePassword";

    public const string PAGE_COURSE = "/Course";
    public const string PAGE_COURSE_DETAIL = "/Course/Detail";

    // static files
    public const string FAVICON = "/img/favicon.png";
}
