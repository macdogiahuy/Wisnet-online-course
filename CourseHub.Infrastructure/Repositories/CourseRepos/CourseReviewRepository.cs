﻿using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseReviewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseHub.Infrastructure.Repositories.CourseRepos;

public class CourseReviewRepository : BaseRepository<CourseReview>, ICourseReviewRepository
{
    public CourseReviewRepository(DbContext context) : base(context)
    {
    }

    public IPagingQuery<CourseReview, CourseReviewModel> GetPagingQuery(Expression<Func<CourseReview, bool>>? whereExpression, short pageIndex, byte pageSize)
    {
        throw new NotImplementedException();
    }
}
