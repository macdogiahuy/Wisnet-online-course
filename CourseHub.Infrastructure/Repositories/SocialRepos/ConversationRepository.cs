using CourseHub.Core.Entities.SocialDomain;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Interfaces.Repositories.SocialRepos;
using CourseHub.Core.Models.Social;
using CourseHub.Core.Services.Mappers.ConversationMappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseHub.Infrastructure.Repositories.SocialRepos;

public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(DbContext context) : base(context)
    {
    }

    public IPagingQuery<Conversation, ConversationModel> GetPagingQuery(Expression<Func<Conversation, bool>>? whereExpression, short pageIndex, byte pageSize)
    {
        return GetPagingQuery<ConversationModel>(
            ConversationMapperProfile.ModelConfig, whereExpression, pageIndex, pageSize,
            includeExpressions: _ => _.Members);
    }
}
