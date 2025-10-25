using AutoMapper;
using CourseHub.Core.Entities.CourseDomain;
using CourseHub.Core.Entities.CourseDomain.Enums;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Course.CourseDtos;
using CourseHub.Core.Services.Domain.CourseServices;
using FluentAssertions;
using CourseHub.Core.RequestDtos.Shared;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace CourseHub.Tests.Domain.Courses;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICourseRepository> _courseRepository = new();
    private readonly Mock<IInstructorRepository> _instructorRepository = new();
    private readonly Mock<ISectionRepository> _sectionRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IAppLogger> _logger = new();

    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _unitOfWork.SetupGet(u => u.CourseRepo).Returns(_courseRepository.Object);
        _unitOfWork.SetupGet(u => u.InstructorRepo).Returns(_instructorRepository.Object);
        _unitOfWork.SetupGet(u => u.SectionRepo).Returns(_sectionRepository.Object);

        _service = new CourseService(_unitOfWork.Object, _mapper.Object, _logger.Object);
    }

    [Fact(DisplayName = "CreateAsync trả về Created khi dữ liệu hợp lệ")]
    public async Task CreateAsync_Should_ReturnCreated_When_InputIsValid()
    {
        var dto = new CreateCourseDto
        {
            Title = "Clean Architecture",
            Thumb = new CreateMediaDto { Url = "thumb.jpg" },
            Intro = "Intro",
            Description = "Description",
            Price = 150,
            Level = CourseLevel.Intermediate,
            Outcomes = "Outcomes",
            Requirements = "Requirements",
            LeafCategoryId = Guid.NewGuid(),
            SectionNames = new List<string> { "Intro", "Basics" }
        };
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Insert(It.IsAny<Course>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        ServiceResult<Guid> result = await _service.CreateAsync(dto, clientId);

        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(201);
        _courseRepository.Verify(r => r.Insert(It.Is<Course>(c =>
            c.Title == dto.Title &&
            c.LeafCategoryId == dto.LeafCategoryId &&
            c.Sections.Count == dto.SectionNames.Count
        )), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact(DisplayName = "CreateAsync trả về lỗi ServerError khi không tìm thấy giảng viên")]
    public async Task CreateAsync_Should_ReturnServerError_When_InstructorMissing()
    {
        var dto = new CreateCourseDto
        {
            Title = "Missing Instructor",
            Thumb = new CreateMediaDto { Url = "thumb.jpg" },
            Intro = "Intro",
            Description = "Description",
            Price = 120,
            Level = CourseLevel.Beginner,
            Outcomes = "Outcomes",
            Requirements = "Requirements",
            LeafCategoryId = Guid.NewGuid(),
            SectionNames = new List<string> { "Intro" }
        };
        var clientId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(Guid.Empty);

        ServiceResult<Guid> result = await _service.CreateAsync(dto, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(500);
        _courseRepository.Verify(r => r.Insert(It.IsAny<Course>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync xử lý ngoại lệ từ repository")]
    public async Task CreateAsync_Should_HandleRepositoryException()
    {
        var dto = new CreateCourseDto
        {
            Title = "DB Exception",
            Thumb = new CreateMediaDto { Url = "thumb.jpg" },
            Intro = "Intro",
            Description = "Description",
            Price = 99,
            Level = CourseLevel.Beginner,
            Outcomes = "Outcomes",
            Requirements = "Requirements",
            LeafCategoryId = Guid.NewGuid(),
            SectionNames = new List<string> { "Intro" }
        };
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Insert(It.IsAny<Course>())).ThrowsAsync(new InvalidOperationException("db error"));

        ServiceResult<Guid> result = await _service.CreateAsync(dto, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(500);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "UpdateAsync trả về ServerError khi không tìm thấy giảng viên")]
    public async Task UpdateAsync_Should_ReturnServerError_When_InstructorMissing()
    {
        var dto = new UpdateCourseDto { Id = Guid.NewGuid() };
        var clientId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(Guid.Empty);

        ServiceResult result = await _service.UpdateAsync(dto, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(500);
        _courseRepository.Verify(r => r.Find(dto.Id), Times.Never);
    }

    [Fact(DisplayName = "UpdateAsync trả về BadRequest khi không tìm thấy khóa học")]
    public async Task UpdateAsync_Should_ReturnBadRequest_When_CourseMissing()
    {
        var dto = new UpdateCourseDto { Id = Guid.NewGuid() };
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Find(dto.Id)).ReturnsAsync((Course?)null);

        ServiceResult result = await _service.UpdateAsync(dto, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(400);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "UpdateAsync cập nhật khóa học và commit thành công")]
    public async Task UpdateAsync_Should_UpdateCourse_When_DataValid()
    {
        var courseId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var existingCourse = BuildCourse(courseId, clientId, instructorId);

        var dto = new UpdateCourseDto
        {
            Id = courseId,
            Title = "Updated Title",
            Intro = "New Intro",
            Description = "New Description",
            Price = 200,
            Discount = 0.2,
            DiscountExpiry = DateTime.UtcNow.AddDays(10),
            Level = CourseLevel.All,
            Outcomes = "Updated Outcomes",
            Requirements = "Updated Requirements",
            LeafCategoryId = Guid.NewGuid(),
            AddedSections = new List<string> { "Advanced" },
            RemovedSections = new List<Guid> { Guid.NewGuid() },
            Thumb = new CreateMediaDto { Url = "new-thumb.jpg" }
        };

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Find(courseId)).ReturnsAsync(existingCourse);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        ServiceResult result = await _service.UpdateAsync(dto, clientId);

        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(200);
        existingCourse.Title.Should().Be(dto.Title);
    existingCourse.Price.Should().Be(dto.Price!.Value);
    existingCourse.Level.Should().Be(dto.Level!.Value);
        existingCourse.Outcomes.Should().Be(dto.Outcomes);
        existingCourse.Requirements.Should().Be(dto.Requirements);
    existingCourse.LeafCategoryId.Should().Be(dto.LeafCategoryId!.Value);
        existingCourse.ThumbUrl.Should().Be(dto.Thumb!.Url);
        existingCourse.LastModifierId.Should().Be(clientId);
    existingCourse.Discount.Should().Be(dto.Discount!.Value);
        existingCourse.DiscountExpiry.Should().BeCloseTo(dto.DiscountExpiry.Value, TimeSpan.FromSeconds(1));

        _sectionRepository.Verify(r => r.RemoveRangeById(courseId, dto.RemovedSections!), Times.Once);
        _courseRepository.Verify(r => r.LoadSections(existingCourse), Times.Once);
        existingCourse.Sections.Should().Contain(s => s.Title == "Advanced");
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync trả về ServerError khi không tìm thấy giảng viên")]
    public async Task DeleteAsync_Should_ReturnServerError_When_InstructorMissing()
    {
        var courseId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(Guid.Empty);

        ServiceResult result = await _service.DeleteAsync(courseId, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(500);
        _courseRepository.Verify(r => r.Find(courseId), Times.Never);
    }

    [Fact(DisplayName = "DeleteAsync trả về NotFound khi khóa học không tồn tại")]
    public async Task DeleteAsync_Should_ReturnNotFound_When_CourseMissing()
    {
        var courseId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Find(courseId)).ReturnsAsync((Course?)null);

        ServiceResult result = await _service.DeleteAsync(courseId, clientId);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(404);
        _courseRepository.Verify(r => r.Delete(It.IsAny<Course>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "DeleteAsync xóa khóa học thành công")]
    public async Task DeleteAsync_Should_DeleteCourse_When_EntityExists()
    {
        var courseId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var existingCourse = BuildCourse(courseId, clientId, instructorId);

        _instructorRepository.Setup(r => r.GetIdByUserId(clientId)).ReturnsAsync(instructorId);
        _courseRepository.Setup(r => r.Find(courseId)).ReturnsAsync(existingCourse);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        ServiceResult result = await _service.DeleteAsync(courseId, clientId);

        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(200);
        _courseRepository.Verify(r => r.Delete(existingCourse), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetPagedAsync sắp xếp theo giá khi ByPrice = true")]
    public async Task GetPagedAsync_Should_OrderByPrice_When_ByPriceEnabled()
    {
        var dto = new QueryCourseDto
        {
            PageIndex = 0,
            PageSize = 10,
            ByPrice = true
        };
        var paged = new PagedResult<CourseOverviewModel>(1, 0, 10, new List<CourseOverviewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course",
                MetaTitle = "course",
                ThumbUrl = "thumb",
                Status = CourseStatus.Ongoing,
                Price = 100,
                Discount = 0,
                DiscountExpiry = DateTime.UtcNow.AddDays(1),
                Level = CourseLevel.Beginner,
                LectureCount = 10,
                LearnerCount = 5,
                RatingCount = 1,
                TotalRating = 5,
                BookmarkCount = 0,
                LastModificationTime = DateTime.UtcNow
            }
        });
        var queryMock = new Mock<IPagingQuery<Course, CourseOverviewModel>>();
        queryMock
            .Setup(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, double>>>(), true, false, false))
            .ReturnsAsync(paged);

        _courseRepository
            .Setup(r => r.GetPagingQuery(It.IsAny<Expression<Func<Course, bool>>>(), dto.PageIndex, dto.PageSize, It.IsAny<Expression<Func<Course, object?>>[]>()))
            .Returns(queryMock.Object);

        ServiceResult<PagedResult<CourseOverviewModel>> result = await _service.GetPagedAsync(dto);

        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be(paged);
        queryMock.Verify(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, double>>>(), true, false, false), Times.Once);
    }

    [Fact(DisplayName = "GetPagedAsync sắp xếp theo Discount giảm dần khi ByDiscount = true")]
    public async Task GetPagedAsync_Should_OrderByDiscountDescending()
    {
        var dto = new QueryCourseDto
        {
            PageIndex = 0,
            PageSize = 10,
            ByDiscount = true
        };
        var paged = new PagedResult<CourseOverviewModel>(1, 0, 10, new List<CourseOverviewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course",
                MetaTitle = "course",
                ThumbUrl = "thumb",
                Status = CourseStatus.Ongoing,
                Price = 100,
                Discount = 0.5,
                DiscountExpiry = DateTime.UtcNow.AddDays(1),
                Level = CourseLevel.Beginner,
                LectureCount = 10,
                LearnerCount = 5,
                RatingCount = 1,
                TotalRating = 5,
                BookmarkCount = 0,
                LastModificationTime = DateTime.UtcNow
            }
        });
        var queryMock = new Mock<IPagingQuery<Course, CourseOverviewModel>>();
        queryMock
            .Setup(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, double>>>(), false, false, false))
            .ReturnsAsync(paged);

        _courseRepository
            .Setup(r => r.GetPagingQuery(It.IsAny<Expression<Func<Course, bool>>>(), dto.PageIndex, dto.PageSize, It.IsAny<Expression<Func<Course, object?>>[]>()))
            .Returns(queryMock.Object);

        ServiceResult<PagedResult<CourseOverviewModel>> result = await _service.GetPagedAsync(dto);

        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be(paged);
        queryMock.Verify(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, double>>>(), false, false, false), Times.Once);
    }

    [Fact(DisplayName = "GetPagedAsync mặc định sắp xếp theo LastModificationTime giảm dần")]
    public async Task GetPagedAsync_Should_OrderByLastModificationTime_When_NoFlag()
    {
        var dto = new QueryCourseDto { PageIndex = 0, PageSize = 10 };
        var paged = new PagedResult<CourseOverviewModel>(1, 0, 10, new List<CourseOverviewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course",
                MetaTitle = "course",
                ThumbUrl = "thumb",
                Status = CourseStatus.Ongoing,
                Price = 100,
                Discount = 0,
                DiscountExpiry = DateTime.UtcNow.AddDays(1),
                Level = CourseLevel.Beginner,
                LectureCount = 10,
                LearnerCount = 5,
                RatingCount = 1,
                TotalRating = 5,
                BookmarkCount = 0,
                LastModificationTime = DateTime.UtcNow
            }
        });
        var queryMock = new Mock<IPagingQuery<Course, CourseOverviewModel>>();
        queryMock
            .Setup(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, DateTime>>>(), false, false, false))
            .ReturnsAsync(paged);

        _courseRepository
            .Setup(r => r.GetPagingQuery(It.IsAny<Expression<Func<Course, bool>>>(), dto.PageIndex, dto.PageSize, It.IsAny<Expression<Func<Course, object?>>[]>()))
            .Returns(queryMock.Object);

        ServiceResult<PagedResult<CourseOverviewModel>> result = await _service.GetPagedAsync(dto);

        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be(paged);
        queryMock.Verify(q => q.ExecuteWithOrderBy(It.IsAny<Expression<Func<Course, DateTime>>>(), false, false, false), Times.Once);
    }

    private static Course BuildCourse(Guid courseId, Guid creatorId, Guid instructorId)
    {
        var course = new Course(
            courseId,
            creatorId,
            instructorId,
            Guid.NewGuid(),
            "Title",
            "thumb",
            "Intro",
            "Description",
            100,
            CourseLevel.Beginner,
            "Outcomes",
            "Requirements",
            new List<Section> { new(0, "Intro") }
        )
        {
            LastModifierId = creatorId
        };

        course.SetDiscount(0, DateTime.UtcNow.AddDays(30));

        return course;
    }
}
