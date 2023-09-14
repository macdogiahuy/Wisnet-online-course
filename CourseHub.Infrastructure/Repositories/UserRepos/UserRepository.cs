using AutoMapper.QueryableExtensions;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Interfaces.Repositories.UserRepos;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.Services.Mappers.UserMappers;
using CourseHub.Infrastructure.AccessContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseHub.Infrastructure.Repositories.UserRepos;

internal class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(Context context) : base(context) { }



    public async Task<UserModel?> GetAsync(Guid id)
    {
        return await DbSet
            .Where(_ => _.Id == id)
            .ProjectTo<UserModel>(UserMapperProfile.UserModelConfig)
            .FirstOrDefaultAsync();
    }

    public async Task<UserFullModel?> GetFullAsync(Guid id)
    {
        return await DbSet
            .Where(_ => _.Id == id)
            .ProjectTo<UserFullModel>(UserMapperProfile.UserFullModelConfig)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserOverviewModel>> GetOverviewAsync(List<Guid> ids)
    {
        return await DbSet
            .Where(_ => ids.Contains(_.Id))
            .ProjectTo<UserOverviewModel>(UserMapperProfile.UserOverviewModelConfig)
            .ToListAsync();
    }

    public IPagingQuery<User, UserModel> GetPagingQuery(Expression<Func<User, bool>>? whereExpression, short pageIndex, byte pageSize)
    {
        return GetPagingQuery<UserModel>(UserMapperProfile.UserModelConfig, whereExpression, pageIndex, pageSize);
    }



    public async Task<bool> EmailExisted(string email) => await Any(_ => _.Email == email);

    public async Task<bool> UserNameExisted(string userName) => await Any(_ => _.UserName == userName);

    public async Task<User?> FindByEmail(string email) => await DbSet.FirstOrDefaultAsync(_ => _.Email == email);

    public async Task<User?> FindByUserName(string userName) => await DbSet.FirstOrDefaultAsync(_ => _.UserName == userName);
}
