using CourseHub.Core.Helpers.Http;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Helpers.Utils;
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

            if (result is null)
                return null;

            if (!ResourceHelper.IsRemote(result.ThumbUrl))
                result.ThumbUrl = Configurer.GetApiClientOptions().ApiServerPath + $"/api/courses/Resource/{result.Id}/local-thumb";
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<CourseOverviewModel?> GetBySectionIdAsync(Guid sectionId)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<CourseOverviewModel>(
                $"api/courses/BySection?sectionId={sectionId}", SerializeOptions.JsonOptions);

            if (result is null)
                return null;

            if (!ResourceHelper.IsRemote(result.ThumbUrl))
                result.ThumbUrl = Configurer.GetApiClientOptions().ApiServerPath + $"/api/courses/Resource/{result.Id}/local-thumb";
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

            foreach (var item in result!.Items)
            {
                if (!ResourceHelper.IsRemote(item.ThumbUrl))
                    item.ThumbUrl = Configurer.GetApiClientOptions().ApiServerPath + $"/api/courses/Resource/{item.Id}/local-thumb";
			}

            return result;
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






    public async Task<HttpResponseMessage> CreateAsync(CreateCourseDto dto, HttpContext context)
    {
        _client.AddBearerHeader(context);
		var result = await _client.PostAsync("/api/courses", ToFormData(dto));
        return result;
	}

    public Task<HttpResponseMessage> UpdateAsync(UpdateCourseDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id, HttpContext context)
    {
        throw new NotImplementedException();
    }






    public async Task<bool> IsEnrolled(Guid courseId)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<bool>($"api/enrollments?courseId={courseId}");
            return result!;
        }
        catch
        {
            return false;
        }
    }

	private MultipartFormDataContent ToFormData(CreateCourseDto dto)
	{
		FormDataHelper helper = new()
		{
			KeyValuePairs = new()
			{
				{ nameof(dto.Title), dto.Title },
				{ "Thumb.Url", dto.Thumb.Url },
				{ nameof(dto.Intro), dto.Intro },
				{ nameof(dto.Description), dto.Description },
				{ nameof(dto.Price), dto.Price.ToString() },
				{ nameof(dto.Level), ((int)dto.Level).ToString() },
				{ nameof(dto.Outcomes), dto.Outcomes },
				{ nameof(dto.Requirements), dto.Requirements },
				{ nameof(dto.LeafCategoryId), dto.LeafCategoryId.ToString() }
			}
		};

		if (dto.SectionNames is not null && dto.SectionNames.Count > 0)
		{
			for (int i = 0; i < dto.SectionNames.Count; i++)
				helper.KeyValuePairs.Add($"SectionNames[{i}]", dto.SectionNames[i]);
		}

        if (dto.Thumb.File is not null)
        {
            helper.Files = new List<(Stream, string, string)>();
            helper.Files.Add((dto.Thumb.File.OpenReadStream(), "Thumb.File", dto.Thumb.File.FileName));
        }

		/*if (dto.Metas is not null)
        {
            for (int i = 0; i < dto.Metas.Count; i++)
                helper.KeyValuePairs.Add($"Metas[{i}]", dto)
        }*/

		return helper.ToFormData();
	}
}
