using AutoMapper;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Models.User.UserModels;

namespace CourseHub.Core.Services.Mappers.UserMappers;

public class UserMapperProfile : Profile
{
    public static readonly IConfigurationProvider UserFullModelConfig = new MapperConfiguration(
        cfg => cfg.CreateMap<User, UserFullModel>()
    );

    public static readonly IConfigurationProvider UserModelConfig = new MapperConfiguration(
        cfg => cfg.CreateMap<User, UserModel>()
    );

    public static readonly IConfigurationProvider UserOverviewModelConfig = new MapperConfiguration(
        cfg => cfg.CreateMap<User, UserOverviewModel>()
    );

    public UserMapperProfile()
    {
        CreateMap<User, UserFullModel>();
    }
}
