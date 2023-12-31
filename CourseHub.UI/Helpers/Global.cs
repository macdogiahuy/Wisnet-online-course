﻿namespace CourseHub.UI.Helpers;

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
    public const string PAGE_USER = "/User";

	public const string PAGE_INSTRUCTOR = "/Instructor";
    public const string PAGE_INSTRUCTOR_REQUEST = "/Instructor/Request";
    public const string PAGE_COURSE = "/Course/Search";
    public const string PAGE_COURSE_DETAIL = "/Course/Detail";
    public const string PAGE_COURSE_MANAGE = "/Course/Manage";
    public const string PAGE_COURSE_CREATE = "/Course/Create";
    public const string PAGE_COURSE_UPDATE = "/Course/Update";
    public const string PAGE_SECTION_UPDATE = "/Section/Update";
    public const string PAGE_LECTURE = "/Lecture/Detail";

    public const string PAGE_PAYMENT = "/Payment/Index";

    public const string PAGE_GROUP = "/Group";
    public const string PAGE_GROUP_CONVERSATION = "/Group/Conversation";



	// Partials
	public const string PARTIAL_COURSE_OVERVIEW = "~/Pages/Course/_PartialCourseOverview.cshtml";
    public const string PARTIAL_COURSE_CAROUSEL = "~/Pages/Course/_PartialCarousel.cshtml";
    public const string PARTIAL_PAGINATION = "~/Pages/Shared/_PartialPagination.cshtml";



    // Parterns
    public const string PATTERN_PAGE_INDEX = "{pageIndex}";
    public const string PATTERN_COURSE_SEARCH = "/Course/Search?page={pageIndex}";



    // static files
    public const string FAVICON = "/img/favicon.png";
}
