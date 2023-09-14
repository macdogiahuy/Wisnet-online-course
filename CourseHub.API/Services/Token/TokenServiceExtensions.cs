using CourseHub.API.Helpers.AppStart;
using CourseHub.Core.Interfaces.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CourseHub.API.Services.Token;

public static class TokenServiceExtensions
{
    public static IServiceCollection AddTokenService(this IServiceCollection services)
    {
        TokenOptions config = Configurer.GetTokenOptions();

        services.Configure<TokenOptions>(options =>
        {
            options.Issuer = config.Issuer;
            options.Audience = config.Audience;
            options.Secret = config.Secret;
            options.Lifetime = config.Lifetime;
        });

        services.AddTransient<ITokenService, TokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(BearerOptionsProvider.GetOptions(config))
            .AddCookie(options =>
            {
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.IsEssential = true;
            });

        return services;
    }
}
