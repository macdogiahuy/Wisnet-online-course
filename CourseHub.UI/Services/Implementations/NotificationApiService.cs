using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Common.NotificationModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Common.NotificationDtos;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts;

namespace CourseHub.UI.Services.Implementations;

public class NotificationApiService : INotificationApiService
{
    private readonly HttpClient _client;

    public NotificationApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<PagedResult<NotificationModel>?> GetPaged(QueryNotificationDto dto, HttpContext context)
    {
        try
        {
            string query = QueryBuilder.Build(dto);
            return await _client.GetFromJsonAsync<PagedResult<NotificationModel>>($"api/notifications?{query}");
        }
        catch
        {
            return null;
        }
    }
}
