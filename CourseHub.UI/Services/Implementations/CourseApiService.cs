using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts;

namespace CourseHub.UI.Services.Implementations;

public class CourseApiService : ICourseApiService
{
    private readonly HttpClient _client;

    public CourseApiService(HttpClient client)
    {
        _client = client;
    }






    public async Task<CourseModel?> GetAsync(Guid id)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<CourseModel>(
                $"api/courses/{id}", SerializeOptions.JsonOptions);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<PagedResult<CourseOverviewModel>> GetPagedAsync(QueryCourseDto dto)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<PagedResult<CourseOverviewModel>>(
                $"api/courses?{QueryBuilder.Build(dto)}", SerializeOptions.JsonOptions);
            return result!;
        }
        catch
        {
            return PagedResult<CourseOverviewModel>.GetEmpty();
        }
    }

    public Task<List<CourseOverviewModel>?> GetMultipleAsync(Guid[] ids)
    {
        throw new NotImplementedException();
    }

    public Task<List<CourseOverviewModel>?> GetSimilarAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<CourseMinModel>?> GetMinAsync(QueryCourseDto id)
    {
        throw new NotImplementedException();
    }






    public Task<HttpResponseMessage> CreateAsync(CreateCourseDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> UpdateAsync(UpdateCourseDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id, HttpContext context)
    {
        throw new NotImplementedException();
    }
}
