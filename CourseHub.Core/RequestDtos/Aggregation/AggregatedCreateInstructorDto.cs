using CourseHub.Core.RequestDtos.Course.InstructorDtos;
using CourseHub.Core.RequestDtos.User.UserDtos;

namespace CourseHub.Core.RequestDtos.Aggregation;

public class AggregatedCreateInstructorDto
{
    public CreateUserDto UserDto { get; set; }
    public CreateInstructorDto InstructorDto { get; set; }
}
