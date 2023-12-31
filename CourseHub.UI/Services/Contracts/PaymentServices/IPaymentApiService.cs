﻿using CourseHub.Core.RequestDtos.Payment.BillDtos;

namespace CourseHub.UI.Services.Contracts.PaymentServices;

// Not IBillApiService
public interface IPaymentApiService
{
    Task<HttpResponseMessage> GetUrl(CreateBillDto dto, HttpContext context);
}
