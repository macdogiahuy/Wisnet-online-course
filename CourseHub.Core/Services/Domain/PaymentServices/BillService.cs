using AutoMapper;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.RequestDtos.Payment.BillDtos;
using CourseHub.Core.Services.Domain.PaymentServices.Contracts;
using CourseHub.Core.Services.Domain.PaymentServices.TempModels;

namespace CourseHub.Core.Services.Domain.PaymentServices;

public class BillService : DomainService, IBillService
{
    public BillService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }

    public Task<ServiceResult<PagedResult<Bill>>> Get(QueryBillDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<Guid>> Create(Guid id, CreateBillDto dto, PaymentResponse response, Guid clientId)
    {
        var entity = Adapt(id, dto, response, clientId);
        try
        {
            await _uow.BillRepo.Insert(entity);
            return Created(entity.Id);
        }
        catch
        {
            return ServerError<Guid>();
        }
    }






    private Bill Adapt(Guid id, CreateBillDto dto, PaymentResponse paymentResponse, Guid clientId)
    {
        return new Bill(
            id, dto.Action, dto.Note, paymentResponse.Amount, dto.Gateway, paymentResponse.TransactionId,
            paymentResponse.ClientTransactionId, paymentResponse.Token, paymentResponse.IsSuccessful, clientId);
    }
}
