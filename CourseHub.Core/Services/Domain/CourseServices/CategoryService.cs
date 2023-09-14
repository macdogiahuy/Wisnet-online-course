using AutoMapper;
using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.RequestDtos.Course.CategoryDtos;

namespace CourseHub.Core.Services.Domain.CourseServices;

public class CategoryService : DomainService, ICategoryService
{
    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }

    public Task<ServiceResult<Guid>> CreateAsync(CreateCategoryDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<List<Category>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> UpdateAsync(UpdateCategoryDto entity)
    {
        throw new NotImplementedException();
    }
}
