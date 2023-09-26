using CourseHub.Core.RequestDtos.Payment.BillDtos;
using CourseHub.UI.Helpers.AppStart;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Services.Contracts;

namespace CourseHub.UI.Services.Implementations;

public class PaymentApiService : IPaymentApiService
{
    private readonly HttpClient _client;

    public PaymentApiService(HttpClient client)
    {
        _client = client;
    }






    public async Task<HttpResponseMessage> GetUrl(CreateBillDto dto, HttpContext context)
    {
        _client.AddBearerHeader(context);
        return await _client.GetAsync($"/api/bills/RedirectLink?{QueryBuilder.Build(dto)}");
    }
}
