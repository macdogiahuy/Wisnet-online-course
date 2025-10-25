using AutoMapper;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Entities.UserDomain.Enums;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Interfaces.Authentication;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.UserRepos;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.Core.Services.Domain.UserServices;
using CourseHub.Core.Services.Domain.UserServices.TempModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace CourseHub.Tests.Domain.Users;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IAppLogger> _logger = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _unitOfWork.SetupGet(u => u.UserRepo).Returns(_userRepository.Object);
        _service = new UserService(_unitOfWork.Object, _mapper.Object, _logger.Object);
    }

    [Fact(DisplayName = "SignInAsync trả về AuthModel khi thông tin hợp lệ")]
    public async Task SignInAsync_Should_ReturnAuthModel_When_CredentialsValid()
    {
        var user = CreateApprovedLearner("johndoe", "Password123!");
        user.IncreaseAccessFailedCount();

        var dto = new SignInDto { UserName = user.UserName, Password = "Password123!" };
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.GenerateAccessToken(user.Id.ToString(), user.Role.ToString())).Returns("access-token");
        tokenService.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        var expectedFullModel = new UserFullModel { Id = user.Id, UserName = user.UserName, Role = user.Role };
        _mapper.Setup(m => m.Map<UserFullModel>(user)).Returns(expectedFullModel);
        _userRepository.Setup(r => r.FindByUserName(user.UserName)).ReturnsAsync(user);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        ServiceResult<AuthModel> result = await _service.SignInAsync(dto, tokenService.Object);

        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");
        result.Data.User.Should().Be(expectedFullModel);
        user.AccessFailedCount.Should().Be(0);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        tokenService.Verify(t => t.GenerateAccessToken(user.Id.ToString(), user.Role.ToString()), Times.Once);
    }

    [Fact(DisplayName = "SignInAsync trả về Unauthorized khi sai mật khẩu")]
    public async Task SignInAsync_Should_ReturnUnauthorized_When_PasswordInvalid()
    {
        var user = CreateApprovedLearner("johndoe", "Password123!");
        var dto = new SignInDto { UserName = user.UserName, Password = "WrongPassword1!" };
        var tokenService = new Mock<ITokenService>();

        _userRepository.Setup(r => r.FindByUserName(user.UserName)).ReturnsAsync(user);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        ServiceResult<AuthModel> result = await _service.SignInAsync(dto, tokenService.Object);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(401);
        user.AccessFailedCount.Should().Be(1);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        tokenService.Verify(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "SignInAsync trả về Forbidden khi tài khoản chưa được duyệt")]
    public async Task SignInAsync_Should_ReturnForbidden_When_UserNotApproved()
    {
        var user = CreateLearnerWithoutApproval("johndoe", "Password123!");
        var dto = new SignInDto { UserName = user.UserName, Password = "Password123!" };
        var tokenService = new Mock<ITokenService>();

        _userRepository.Setup(r => r.FindByUserName(user.UserName)).ReturnsAsync(user);

        ServiceResult<AuthModel> result = await _service.SignInAsync(dto, tokenService.Object);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(403);
        tokenService.Verify(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "SignInAsync trả về Forbidden khi vượt quá số lần đăng nhập thất bại")]
    public async Task SignInAsync_Should_ReturnForbidden_When_AccessFailedExceeded()
    {
        var user = CreateApprovedLearner("johndoe", "Password123!");
        for (int i = 0; i < 6; i++)
            user.IncreaseAccessFailedCount();
        var dto = new SignInDto { UserName = user.UserName, Password = "Password123!" };
        var tokenService = new Mock<ITokenService>();

        _userRepository.Setup(r => r.FindByUserName(user.UserName)).ReturnsAsync(user);

        ServiceResult<AuthModel> result = await _service.SignInAsync(dto, tokenService.Object);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(403);
        tokenService.Verify(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact(DisplayName = "SignInAsync trả về BadRequest khi thiếu thông tin đăng nhập")]
    public async Task SignInAsync_Should_ReturnBadRequest_When_CredentialsMissing()
    {
        var dto = new SignInDto { Password = "Password123!" };
        var tokenService = new Mock<ITokenService>();

        ServiceResult<AuthModel> result = await _service.SignInAsync(dto, tokenService.Object);

        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(400);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    private static User CreateApprovedLearner(string userName, string password)
    {
        var user = new User(userName, password, $"{userName}@example.com", Role.Learner);
        user.Approve();
        return user;
    }

    private static User CreateLearnerWithoutApproval(string userName, string password)
    {
        return new User(userName, password, $"{userName}@example.com", Role.Learner);
    }
}
