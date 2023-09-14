﻿using AutoMapper;
using CourseHub.Core.Entities.UserDomain;
using CourseHub.Core.Entities.UserDomain.Enums;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Interfaces.Authentication;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.Shared;
using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.User.UserDtos;
using CourseHub.Core.Services.Domain.UserServices.TempModels;
using CourseHub.Core.Services.Storage;
using CourseHub.Core.Services.Storage.Utils;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CourseHub.Core.Services.Domain.UserServices;

public class UserService : DomainService, IUserService
{
    private const byte MAX_ACCESS_FAILED_COUNT = 5;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger logger) : base(unitOfWork, mapper, logger)
    {
    }






    public async Task<ServiceResult<UserFullModel>> GetFullAsync(Guid id)
    {
        var result = await _uow.UserRepo.GetFullAsync(id);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<UserModel>> GetAsync(Guid id)
    {
        var result = await _uow.UserRepo.GetAsync(id);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<PagedResult<UserModel>>> GetPagedAsync(QueryUserDto dto)
    {
        var query = _uow.UserRepo.GetPagingQuery(GetPredicate(dto), dto.PageIndex, dto.PageSize);
        var result = await query.ExecuteWithOrderBy(_ => _.UserName);
        return ToQueryResult(result);
    }

    public async Task<ServiceResult<List<UserOverviewModel>>> GetOverviewAsync(List<Guid> ids)
    {
        var result = await _uow.UserRepo.GetOverviewAsync(ids);
        return ToQueryResult(result);
    }



    /// <summary>
    /// All Registration needs verification (for Users : verify email, for Admins : need approval)
    /// </summary>
    public async Task<ServiceResult<string>> CreateAsync(CreateUserDto dto)
    {
        if (dto.Role == Role.SysAdmin)
            return Forbidden<string>(UserDomainMessages.FORBIDDEN_NOT_APPROVED);
        if (await _uow.UserRepo.EmailExisted(dto.Email))
            return Conflict<string>(UserDomainMessages.CONFLICT_EMAIL);

        User newUser = Adapt(dto);
        try
        {
            await _uow.UserRepo.Insert(newUser);
            await _uow.CommitAsync();
            return Created(newUser.Token);
        }
        catch (Exception ex)
        {
            _logger.Warn(ex.Message);
            return ServerError<string>(string.Empty);
        }
    }

    public async Task<ServiceResult<UserFullModel>> UpdateAsync(UpdateUserDto dto, Guid? userId)
    {
        if (userId is null)
            return Unauthorized<UserFullModel>();
        User? entity = await _uow.UserRepo.Find(userId);
        if (entity is null)
            return Unauthorized<UserFullModel>();

        if (dto.CurrentPassword is not null)
        {
            if (dto.NewPassword is null)
                return BadRequest<UserFullModel>(UserDomainMessages.INVALID_NEWPASSWORD_MISSING);
            if (User.IsMatchPasswords(dto.NewPassword, entity.Password))
                return Unauthorized<UserFullModel>(UserDomainMessages.UNAUTHORIZED_PASSWORD);
        }

        try
        {
            await ApplyChanges(dto, entity);
            await _uow.CommitAsync();
            return Ok(_mapper.Map<UserFullModel>(entity));
        }
        catch (Exception ex)
        {
            _logger.Warn(ex.Message);
            return ServerError<UserFullModel>();
        }
    }



    public async Task<ServiceResult> VerifyAsync(VerifyEmailDto dto)
    {
        var entity = await _uow.UserRepo.FindByEmail(dto.Email);
        if (entity is null)
            return Unauthorized();

        if (entity.Token == dto.Token)
            entity.Approve();
        await _uow.CommitAsync();
        return Ok();
    }

    public async Task<ServiceResult<AuthDto>> SignInAsync(SignInDto dto, ITokenService tokenService)
    {
        User? entity;
        if (dto.Username is not null)
            entity = await _uow.UserRepo.FindByUserName(dto.Username);
        else if (dto.Email is not null)
            entity = await _uow.UserRepo.FindByEmail(dto.Email);
        else
            return BadRequest<AuthDto>(UserDomainMessages.INVALID_EMAILPHONE_MISSING);

        if (entity is null)
            return Unauthorized<AuthDto>(UserDomainMessages.UNAUTHORIZED_SIGNIN);
        if (entity.AccessFailedCount > MAX_ACCESS_FAILED_COUNT)
            return Forbidden<AuthDto>(UserDomainMessages.FORBIDDEN_FAILED_EXCEED);
        if (entity.IsNotApproved())
            return Forbidden<AuthDto>(UserDomainMessages.FORBIDDEN_NOT_APPROVED);

        if (!User.IsMatchPasswords(dto.Password, entity.Password))
        {
            entity.IncreaseAccessFailedCount();
            await _uow.CommitAsync();
            return Unauthorized<AuthDto>(UserDomainMessages.UNAUTHORIZED_SIGNIN);
        }

        entity.ResetAccessFailedCount();
        AuthDto authDTO = UpdateToken(tokenService, entity);
        authDTO.User = _mapper.Map<UserFullModel>(entity);
        await _uow.CommitAsync();
        return Ok(authDTO);
    }

    public async Task<ServiceResult<AuthDto>> RefreshAsync(string? accessToken, string? refreshToken, ITokenService tokenService)
    {
        if (accessToken is null || refreshToken is null)
            return Unauthorized<AuthDto>("Missing credentials");
        var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
        // not expired -> invalid
        if (principal is null)
            return Unauthorized<AuthDto>("Invalid access token");

        User? user = await FindByClaims(principal);
        if (user is null)
            return Unauthorized<AuthDto>("Invalid access token");
        if (user.RefreshToken != refreshToken)
            return Unauthorized<AuthDto>("Invalid refresh token");

        AuthDto authDTO = UpdateToken(tokenService, user);
        // no need user
        await _uow.CommitAsync();
        return Ok(authDTO);
    }

    public async Task<ServiceResult<string>> GetPasswordResetTokenAsync(string email)
    {
        User? user = await _uow.UserRepo.FindByEmail(email);
        if (user is null)
            return Unauthorized<string>(UserDomainMessages.INVALID_EMAIL);

        user.GenerateToken();
        await _uow.CommitAsync();
        return Ok(user.Token);
    }

    public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        User? user = await _uow.UserRepo.FindByEmail(dto.Email);
        if (user is null || user.Token != dto.Token)
            return BadRequest(UserDomainMessages.INVALID_RESETPASSWORD_ATTEMPT);

        try
        {
            user.SetPassword(dto.NewPassword);
            await _uow.CommitAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.Warn(ex.Message);
            return ServerError();
        }
    }






    private static User Adapt(CreateUserDto dto)
    {
        var entity = new User(dto.UserName, dto.Password)
        {
            Email = dto.Email
        };
        return entity;
    }

    private async Task ApplyChanges(UpdateUserDto dto, User user)
    {
        if (dto.FullName is not null)
            user.SetFullName(dto.FullName);
        if (dto.Phone is not null)
            user.Phone = dto.Phone;
        if (dto.DateOfBirth is not null)
            user.DateOfBirth = (DateTime)dto.DateOfBirth;
        if (dto.Avatar is not null)
            await SaveAvatar(dto.Avatar, user.Id);

        if (dto.CurrentPassword is not null && dto.NewPassword is not null)
            user.SetPassword(dto.NewPassword);
    }

    private Expression<Func<User, bool>>? GetPredicate(QueryUserDto dto)
    {
        return null;
    }

    private async Task<User?> FindByClaims(ClaimsPrincipal userClaim)
    {
        Guid? id = GetIdentifier(userClaim);
        if (id is null)
            return null;
        return await _uow.UserRepo.Find(id);
    }

    private static Guid? GetIdentifier(ClaimsPrincipal userClaim)
    {
        foreach (Claim claim in userClaim.Claims)
            if (claim.Type == ClaimTypes.NameIdentifier)
                return Guid.TryParse(claim.Value, out Guid result) ? result : null;
        return null;
    }

    private AuthDto UpdateToken(ITokenService tokenService, User user)
    {
        string accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Role.ToString());
        string refreshToken = tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken);
        return new AuthDto(_mapper.Map<UserFullModel>(user), accessToken, refreshToken);
    }

    private async Task SaveAvatar(IFormFile file, Guid userId)
    {
        Stream stream = await new FileConverter().ToJpg(file);
        await ServerStorage.SaveFile(stream, UserStorage.GetAvatarPath(userId), _logger);
    }
}
