using CourseHub.Core.Entities.CommonDomain.Enums;
using CourseHub.Core.Entities.CourseDomain.Enums;
using CourseHub.Core.Models.User.UserModels;

namespace CourseHub.Core.Models.Common.CommentModels;

public class CommentModel
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; protected set; }
    public DateTime LastModificationTime { get; protected set; }
    public Guid CreatorId { get; set; }
    public Guid LastModifierId { get; set; }

    public string Content { get; set; }
    public CommentStatus Status { get; set; }
    public CommentSourceEntityType SourceType { get; private set; }

    // FKs
    public Guid? ParentId { get; set; }
    public Guid? LectureId { get; set; }
    public Guid? ArticleId { get; set; }

    // Navigations
    public UserMinModel? Creator { get; set; }
    public Comment? Parent { get; private set; }
    public Lecture? Lecture { get; set; }
    public Article? Article { get; set; }
    public List<CommentMedia> Medias { get; private set; }
    public List<Comment> Replies { get; private set; }
    public List<Reaction> Reactions { get; private set; }
}
