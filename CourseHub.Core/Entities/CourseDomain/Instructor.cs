namespace CourseHub.Core.Entities.CourseDomain;

#pragma warning disable CS8618

/// <summary>
/// Instructor info, does not extend User
/// </summary>
public class Instructor : TimeAuditedEntity
{
    // Attributes
    public string Intro { get; set; }
    public string Experience { get; set; }
    // public string Qualifications { get; set; }

    // FKs
    public Guid CreatorId { get; set; }
     
    // Navigations
    public User? Creator { get; set; }
    public ICollection<Course> Courses { get; private set; }



    public Instructor(string intro, string experience, Guid creatorId)
    {
        Intro = intro;
        Experience = experience;
        CreatorId = creatorId;
    }
}

#pragma warning restore CS8618
