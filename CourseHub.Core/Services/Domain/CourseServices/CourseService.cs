using AutoMapper;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.Core.Services.Storage;
using CourseHub.Core.Services.Storage.Utils;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace CourseHub.Core.Services.Domain.CourseServices;

public class CourseService : DomainService, ICourseService
{
    public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }






    public async Task<ServiceResult<CourseModel>> GetAsync(Guid id)
    {
        var result = await _uow.CourseRepo.GetAsync(id);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<List<CourseMinModel>>> GetMin(QueryCourseDto id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<PagedResult<CourseOverviewModel>>> GetMultiple(Guid[] ids)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<PagedResult<CourseOverviewModel>>> GetPagedAsync(QueryCourseDto dto)
    {
        var query = _uow.CourseRepo.GetPagingQuery(GetPredicate(dto), dto.PageIndex, dto.PageSize);
        var result = await query.ExecuteWithOrderBy(_ => _.LastModificationTime);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<List<CourseOverviewModel>>> GetSimilar(Guid id)
    {
        var result = await _uow.CourseRepo.GetSimilar(id);
        return ToQueryResult(result);
    }






    public async Task<ServiceResult<Guid>> CreateAsync(CreateCourseDto dto, Guid client)
    {
        try
        {
            var instructorId = await _uow.InstructorRepo.GetIdByUserId(client);
            if (instructorId == default)
                return ServerError<Guid>();

            var entity = await Adapt(dto, client, instructorId);
            await _uow.CourseRepo.Insert(entity);
            await _uow.CommitAsync();
            return Created(entity.Id);
        }
        catch (Exception ex)
        {
            return ServerError<Guid>(/*ex.InnerException is null ? ex.Message : ex.InnerException.Message*/);
        }
    }

    public Task<ServiceResult> UpdateAsync(UpdateCourseDto dto, Guid client)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(Guid id, Guid client)
    {
        throw new NotImplementedException();
    }






    private Expression<Func<Course, bool>>? GetPredicate(QueryCourseDto dto)
    {
        return null;
    }

    private async Task<Course> Adapt(CreateCourseDto dto, Guid creatorId, Guid instructorId)
    {
        Guid id = Guid.NewGuid();

        string thumbUrl = string.Empty;
        if (dto.Thumb.File is not null)
            thumbUrl = await SaveThumb(dto.Thumb.File, id);
        else if (dto.Thumb.Url is not null)
            thumbUrl = dto.Thumb.Url;

        var sections = dto.SectionNames.Select((_, index) => new Section((byte)index, _)).ToList();

        return new Course(id, creatorId, instructorId, dto.LeafCategoryId,
            dto.Title, thumbUrl, dto.Intro, dto.Description, dto.Price,
            dto.Level, dto.Outcomes, dto.Requirements, sections);
    }

    private async Task<string> SaveThumb(IFormFile file, Guid courseId)
    {
        Stream stream = await new FileConverter().ToJpg(file);
        string path = CourseStorage.GetCourseThumbPath(courseId);
        await ServerStorage.SaveFile(stream, path, _logger);
        return path;
    }
}
