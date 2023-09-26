using CourseHub.Core.Entities.SocialDomain;
using CourseHub.Core.Interfaces.Repositories.SocialRepos;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.SocialRepos;

public class ChatMessageRepository : BaseRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(DbContext context) : base(context)
    {
    }
}
