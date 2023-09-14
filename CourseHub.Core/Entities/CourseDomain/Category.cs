using CourseHub.Core.Entities.Contracts;

namespace CourseHub.Core.Entities.CourseDomain
{
#pragma warning disable CS8618

    public class Category : Entity
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsLeaf { get; set; }
    }

#pragma warning restore CS8618
}