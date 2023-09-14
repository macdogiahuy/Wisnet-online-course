using CourseHub.Core.Entities.CommonDomain.Enums;
using CourseHub.Core.Entities.Contracts;

namespace CourseHub.Core.Entities.CommonDomain;

public class CommentMedia : DomainObject
{
    public CommentMediaType Type { get; set; }
    public string Url { get; set; }
}
