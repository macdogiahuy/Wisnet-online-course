namespace CourseHub.Core.Entities.CourseDomain;

#pragma warning disable CS8618

public class Lecture : TimeAuditedEntity
{
    // Attributes
    public string Title { get; set; }
    public string Content { get; set; }

    // Navigations
    public List<LectureMaterial> Materials { get; set; }
    public List<Comment> Comments { get; set; }

#pragma warning restore CS8618

    public Lecture()
    {

    }

    public Lecture(string title, string content, List<LectureMaterial> materials)
    {
        Title = title;
        Content = content;
        Materials = materials;
    }
}