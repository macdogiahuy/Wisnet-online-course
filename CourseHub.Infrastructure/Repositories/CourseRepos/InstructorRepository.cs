using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.InstructorModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseHub.Infrastructure.Repositories.CourseRepos;

internal class InstructorRepository : BaseRepository<Instructor>, IInstructorRepository
{
    public InstructorRepository(DbContext context) : base(context)
    {
    }

    

    public async Task<Guid> GetIdByUserId(Guid userId)
    {
        return await DbSet.Where(_ => _.CreatorId == userId).Take(1).Select(_ => _.Id).FirstOrDefaultAsync();
    }

    public Task<InstructorModel?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public IPagingQuery<Instructor, InstructorModel> GetPagingQuery(Expression<Func<User, bool>>? whereExpression, short pageIndex, byte pageSize)
    {
        throw new NotImplementedException();
    }
}
