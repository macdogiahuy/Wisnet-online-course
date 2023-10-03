using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.Social;
using CourseHub.Core.RequestDtos.Social.ConversationDtos;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Helpers.Utils;
using CourseHub.UI.Services.Contracts.SocialServices;

namespace CourseHub.UI.Services.Implementations.SocialServices;

public class ConversationApiService : IConversationApiService
{
    private readonly HttpClient _client;

    public ConversationApiService(HttpClient client)
    {
        _client = client;
    }



    public async Task<PagedResult<ConversationModel>> GetAsync(QueryConversationDto dto, HttpContext context)
    {
        try
        {
            _client.AddBearerHeader(context);
            var result = await _client.GetFromJsonAsync<PagedResult<ConversationModel>>(
                $"api/conversations?{QueryBuilder.Build(dto)}", SerializeOptions.JsonOptions);

            foreach (var item in result!.Items)
            {
                if (string.IsNullOrEmpty(item.AvatarUrl))
                    continue;
                if (!ResourceHelper.IsRemote(item.AvatarUrl))
                    item.AvatarUrl = Configurer.GetApiClientOptions().ApiServerPath + $"/api/conversations/Resource/{item.Id}/local-thumb";
            }

            return result;
        }
        catch
        {
            return PagedResult<ConversationModel>.GetEmpty();
        }
    }

    public Task<HttpResponseMessage> CreateAsync(CreateConversationDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> UpdateAsync(UpdateConversationDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id, HttpContext context)
    {
        throw new NotImplementedException();
    }
}
