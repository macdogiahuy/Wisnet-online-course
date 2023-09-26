using CourseHub.Core.Entities.SocialDomain;
using CourseHub.Core.Interfaces.Repositories.SocialRepos;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.SocialRepos;

public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(DbContext context) : base(context)
    {
    }
}
