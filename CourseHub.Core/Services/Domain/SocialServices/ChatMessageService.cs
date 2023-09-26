using AutoMapper;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Services.Domain.SocialServices.Contracts;

namespace CourseHub.Core.Services.Domain.SocialServices;

public class ChatMessageService : DomainService, IChatMessageService
{
    public ChatMessageService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }
}
