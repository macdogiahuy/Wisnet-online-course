using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.UI.Helpers.Utils;
using CourseHub.UI.Services.Contracts;

namespace CourseHub.UI.Services.Implementations;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _client;

    public UserApiService(HttpClient client)
    {
        _client = client;
    }






    public Task<UserModel?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserFullModel?> GetClientAsync(HttpContext context)
    {
        try
        {
            _client.AddBearerHeader(context);
            return await _client.GetFromJsonAsync<UserFullModel>("api/users/client");
        }
        catch
        {
            return null;
        }
    }

    public Task<List<UserOverviewModel>> GetOverviewAsync(IEnumerable<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<UserModel>> GetPagedAsync(QueryUserDto dto, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public string GetAvatarApiUrl(Guid? id)
    {
        throw new NotImplementedException();
    }






    public async Task<HttpResponseMessage> SignInAsync(SignInDto dto)
    {
        return await _client.PostAsJsonAsync("api/auth/SignIn", dto);
    }

    public Task<HttpResponseMessage> RefreshAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> RegisterAsync(CreateUserDto dto)
    {
        return await _client.PostAsJsonAsync($"api/users", dto);
    }

    public async Task<HttpResponseMessage> VerifyEmailAsync(string email, string token)
    {
        return await _client.PostAsJsonAsync("api/users/verify", new { email, token });
    }

    public async Task<HttpResponseMessage> UpdateAsync(UpdateUserDto dto, HttpContext context)
    {
        Dictionary<string, string?> kvps = new()
        {
            { nameof(dto.FullName), dto.FullName },
            { nameof(dto.DateOfBirth), dto.DateOfBirth?.ToString() },
            { nameof(dto.Bio), dto.Bio },
            { nameof(dto.Phone), dto.Phone },

            { nameof(dto.CurrentPassword), dto.CurrentPassword },
            { nameof(dto.NewPassword), dto.NewPassword }
        };

        MultipartFormDataContent content = new();
        if (dto.Avatar is not null)
        {
            StreamContent sContent = new(dto.Avatar.OpenReadStream());
            sContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.Avatar.ContentType);
            content.Add(sContent, nameof(dto.Avatar), dto.Avatar.FileName);
        }
        foreach (var pair in kvps)
        {
            if (pair.Value is not null)
                content.Add(new StringContent(pair.Value), pair.Key);
        }

        _client.AddBearerHeader(context);
        var response = await _client.PatchAsync("api/users", content);
        if (response.IsSuccessStatusCode)
            context.SetClientData(await response.Content.ReadAsStringAsync());
        return response;
    }

    public async Task<HttpResponseMessage> RequestPasswordResetAsync(string email)
    {
        return await _client.PostAsJsonAsync($"api/users/ForgotPassword", email);
    }

    public async Task<HttpResponseMessage> ResetPasswordAsync(ResetPasswordDto dto)
    {
        return await _client.PostAsJsonAsync("api/users/ResetPassword", dto);
    }
}
