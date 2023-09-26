using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.RequestDtos.Payment.BillDtos;
using CourseHub.Core.Services.Domain.PaymentServices.TempModels;

namespace CourseHub.Core.Services.Domain.PaymentServices.Contracts;

public interface IBillService
{
    Task<ServiceResult<PagedResult<Bill>>> Get(QueryBillDto dto);

    /// <summary>
    /// Not commited operation
    /// </summary>
    Task<ServiceResult<Guid>> Create(Guid id, CreateBillDto dto, PaymentResponse response, Guid clientId);
}
