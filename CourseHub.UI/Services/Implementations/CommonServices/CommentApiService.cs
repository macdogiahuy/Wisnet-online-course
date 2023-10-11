using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Common.CommentModels;
using CourseHub.Core.RequestDtos.Common.CommentDtos;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.CommonServices;

namespace CourseHub.UI.Services.Implementations.CommonServices;

public class CommentApiService : ICommentApiService
{
    private readonly HttpClient _client;

    public CommentApiService(HttpClient client)
    {
        _client = client;
    }



    public async Task<PagedResult<CommentModel>> GetAsync(QueryCommentDto dto)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<PagedResult<CommentModel>>(
                $"api/comments?{QueryBuilder.Build(dto)}", SerializeOptions.JsonOptions);
            return result;
        }
        catch
        {
            return PagedResult<CommentModel>.GetEmpty();
        }
    }

    public Task<HttpResponseMessage> CreateAsync(CreateCommentDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> UpdateAsync(UpdateCommentDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id, HttpContext context)
    {
        throw new NotImplementedException();
    }
}
