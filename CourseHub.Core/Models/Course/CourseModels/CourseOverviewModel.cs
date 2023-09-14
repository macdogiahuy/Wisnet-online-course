namespace CourseHub.Core.Models.Course.CourseModels;

public class CourseOverviewModel
{
    public string ThumbUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int RatingCount { get; set; }
    public int TotalRating { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; }
    //public List<string> Tags { get; set; }
}
