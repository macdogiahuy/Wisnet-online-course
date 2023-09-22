﻿namespace CourseHub.Core.Helpers.Messaging.Messages;

internal static class CourseDomainMessages
{
    // 400
    internal const string INVALID_PARENT = "Invalid parent";
    internal const string INVALID_USER = "Invalid user";
    internal const string INVALID_DISCOUNT = "ByDiscount must be between 0 and 100%";
    internal const string INVALID_DISCOUNT_EXPIRY = "Invalid discount expiration date";
    internal const string INVALID_SECTION = "Invalid section";
    internal const string INVALID_LECTURE = "Invalid lecture";
}