using AutoMapper;
using CourseHub.Core.Models.Common.CommentModels;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Models.Course.CourseReviewModels;
using CourseHub.Core.Models.Course.InstructorModels;
using CourseHub.Core.Models.Course.LectureModels;
using CourseHub.Core.Models.Course.SectionModels;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;

namespace CourseHub.Core.Services.Mappers.CourseMappers;

public class CourseMapperProfile : Profile
{
    public static readonly IConfigurationProvider ModelConfig = new MapperConfiguration(
        cfg =>
        {
            cfg.CreateMap<User, UserModel>();
            cfg.CreateMap<Instructor, InstructorModel>();
            cfg.CreateMap<CourseReview, CourseReviewModel>();
            cfg.CreateMap<Comment, CommentModel>();

            cfg.CreateMap<Lecture, LectureModel>();
            cfg.CreateMap<Section, SectionModel>();
            cfg.CreateMap<Course, CourseModel>();
        }
    );

    public static readonly IConfigurationProvider OverviewModelConfig = new MapperConfiguration(
        cfg =>
        {
            cfg.CreateMap<User, UserMinModel>();
            cfg.CreateMap<Course, CourseOverviewModel>();
        }
    );

    public static readonly IConfigurationProvider MinModelConfig = new MapperConfiguration(
        cfg => cfg.CreateMap<Course, CourseMinModel>()
    );

    public CourseMapperProfile()
    {
        CreateMap<UpdateCourseDto, Course>()
            .ForSourceMember(_ => _.Thumb, act => act.DoNotValidate())
            /*.ForSourceMember(_ => _.RemovedMetas, act => act.DoNotValidate())
            .ForSourceMember(_ => _.AddedMetas, act => act.DoNotValidate())
            .ForSourceMember(_ => _.RemovedSections, act => act.DoNotValidate())
            .ForSourceMember(_ => _.AddedSections, act => act.DoNotValidate())*/;
    }
}
