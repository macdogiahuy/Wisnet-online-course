using CourseHub.Core.Entities.CourseDomain;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts;

namespace CourseHub.UI.Services.Implementations;

public class CategoryApiService : ICategoryApiService
{
    private readonly HttpClient _client;

    public CategoryApiService(HttpClient client)
    {
        _client = client;
    }



    public async Task<List<Category>> GetAsync()
    {
        try
        {
            var result = await _client.GetFromJsonAsync<List<Category>>(
                $"api/categories", SerializeOptions.JsonOptions);
            return result!;
        }
        catch
        {
            return new List<Category>();
        }
    }
}
