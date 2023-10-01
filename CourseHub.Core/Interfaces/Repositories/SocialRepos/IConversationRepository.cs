using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Social;
using System.Linq.Expressions;

namespace CourseHub.Core.Interfaces.Repositories.SocialRepos;

public interface IConversationRepository : IRepository<Conversation>
{
    IPagingQuery<Conversation, ConversationModel> GetPagingQuery(Expression<Func<Conversation, bool>>? whereExpression, short pageIndex, byte pageSize);
}
