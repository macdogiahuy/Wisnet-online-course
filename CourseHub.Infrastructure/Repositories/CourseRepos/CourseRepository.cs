using AutoMapper.QueryableExtensions;
using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.Services.Mappers.CourseMappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseHub.Infrastructure.Repositories.CourseRepos;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(DbContext context) : base(context)
    {
    }

    public async Task<CourseModel?> GetAsync(Guid id)
    {
        return await DbSet
			.AsNoTracking()
			.Where(_ => _.Id == id)
            .Include(_ => _.Creator)
            .Include(_ => _.Sections)
            .Include(_ => _.Metas)
            .Include(_ => _.Reviews)
            .ProjectTo<CourseModel>(CourseMapperProfile.ModelConfig)
            .FirstOrDefaultAsync();
    }

    public IPagingQuery<Course, CourseOverviewModel> GetPagingQuery(
        Expression<Func<Course, bool>>? whereExpression, short pageIndex, byte pageSize,
        params Expression<Func<Course, object?>>[]? includeExpressions)
    {
        if (includeExpressions is null)
            return GetPagingQuery<CourseOverviewModel>(
                CourseMapperProfile.OverviewModelConfig, whereExpression, pageIndex, pageSize);

        return GetPagingQuery<CourseOverviewModel>(
            CourseMapperProfile.OverviewModelConfig, whereExpression, pageIndex, pageSize,
            includeExpressions: includeExpressions);
    }

    public Task<List<CourseOverviewModel>> GetSimilar(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<CourseMinModel?> GetMinAsync(Guid id)
    {
        return await DbSet
            .Where(_ => _.Id == id)
            .ProjectTo<CourseMinModel>(CourseMapperProfile.MinModelConfig)
            .FirstOrDefaultAsync();
    }




    public void LoadSections(Course course) => Context.Entry(course).Collection(p => p.Sections).Load();
}
