using CourseHub.Core.Services.Domain.UserServices;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts.CommonServices;
using CourseHub.UI.Services.Contracts.CourseServices;
using CourseHub.UI.Services.Contracts.PaymentServices;
using CourseHub.UI.Services.Contracts.UserServices;
using CourseHub.UI.Services.Implementations.CommonServices;
using CourseHub.UI.Services.Implementations.CourseServices;
using CourseHub.UI.Services.Implementations.PaymentServices;
using CourseHub.UI.Services.Implementations.UserServices;

namespace CourseHub.UI.Services;

public static class ApiServicesExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        ApiClientOptions options = Configurer.GetApiClientOptions();

        void config(HttpClient client) => client.BaseAddress = new Uri(options.ApiServerPath);
        services.AddTransient<RefreshTokenHandler>();

        services
            .AddDomainServiceWithClient<IUserApiService, UserApiService>(config)

            .AddDomainServiceWithClient<INotificationApiService, NotificationApiService>(config)
            .AddDomainServiceWithClient<ICommentApiService, CommentApiService>(config)

            .AddDomainServiceWithClient<IInstructorApiService, InstructorApiService>(config)
            .AddDomainServiceWithClient<ICategoryApiService, CategoryApiService>(config)
            .AddDomainServiceWithClient<ICourseApiService, CourseApiService>(config)
            .AddDomainServiceWithClient<ILectureApiService, LectureApiService>(config)
            .AddDomainServiceWithClient<ICourseReviewApiService, CourseReviewApiService>(config)

            .AddDomainServiceWithClient<IPaymentApiService, PaymentApiService>(config);

        return services;
    }

    private static IServiceCollection AddDomainServiceWithClient<TService, TImplementation>(this IServiceCollection services, Action<HttpClient> config)
            where TService : class
            where TImplementation : class, TService
    {
        services.AddScoped<TService, TImplementation>();
        services.AddHttpClient<TService, TImplementation>(config).AddHttpMessageHandler<RefreshTokenHandler>();
        return services;
    }
}
