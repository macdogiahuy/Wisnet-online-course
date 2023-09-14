using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.RequestDtos.Course.CategoryDtos;

namespace CourseHub.Core.Services.Domain.CourseServices;

public interface ICategoryService
{
    Task<ServiceResult<List<Category>>> GetAllAsync();

    Task<ServiceResult<Guid>> CreateAsync(CreateCategoryDto entity);
    Task<ServiceResult> UpdateAsync(UpdateCategoryDto entity);
    Task<ServiceResult> DeleteAsync(Guid id);
}
