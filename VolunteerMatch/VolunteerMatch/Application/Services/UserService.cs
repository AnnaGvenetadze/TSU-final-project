using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Application.Interfaces;


namespace VolunteerMatch.Application.Services
{
    public class UserService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IVolunteerTagService _volunteerTagService;
        private readonly ITagValidator _tagValidator;
        private readonly IVolunteerProfileSelectionService _volunteerProfileSelectionService;

        public UserService(
            VolunteerMatchingDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration config,
            IMapper mapper,
            IVolunteerTagService volunteerTagService,
            ITagValidator tagValidator,
            IVolunteerProfileSelectionService volunteerProfileSelectionService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _volunteerTagService = volunteerTagService ??
                throw new ArgumentNullException(nameof(volunteerTagService));
            _tagValidator = tagValidator ??
                throw new ArgumentNullException(nameof(tagValidator));
            _volunteerProfileSelectionService = volunteerProfileSelectionService ??
                throw new ArgumentNullException(nameof(volunteerProfileSelectionService));
        }


        public async Task<AuthResponseDto> RegisterVolunteerAsync(CreateVolunteerDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);
            VolunteerProfileValidator.ValidateForCreate(createDto);

            var user = CreateUser(createDto.Email, createDto.Password, UserRoles.Volunteer);
            var profile = _mapper.Map<VolunteerProfile>(createDto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await _tagValidator
                    .ValidateSelectedTagIdsAsync(createDto.SelectedTagIds);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.VolunteerId = user.UserId;
                _context.VolunteerProfiles.Add(profile);

                await _volunteerProfileSelectionService
                    .SaveVolunteerSkillsAndInterestsAsync(
                        profile,
                        createDto.SelectedSkillIds,
                        createDto.SelectedInterestIds);

                await _volunteerTagService
                    .SaveVolunteerTagsAsync(user.UserId, createDto.SelectedTagIds);

                await _context.SaveChangesAsync();
                var response = await CreateAuthResponseAsync(user);
                // რეფრეშ ტოკენი თუ არ დაინსერთდა ვერ დავარეგისტრირებთ,
                // ესეც ამ ტრანზაქციაში უნდა
                await tx.CommitAsync();

                return response;
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<AuthResponseDto> RegisterOrganizationAsync(CreateOrganizationDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var user = CreateUser(createDto.Email, createDto.Password, UserRoles.Organization);
            var profile = _mapper.Map<OrganizationProfile>(createDto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.OrganizationId = user.UserId;
                _context.OrganizationProfiles.Add(profile);

                await _context.SaveChangesAsync();
                var response = await CreateAuthResponseAsync(user);

                await tx.CommitAsync();

                return response;
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
        }


        public async Task<AuthResponseDto> LoginUserAsync(LoginUserDto loginDto)
        {
            ArgumentNullException.ThrowIfNull(loginDto);

            var email = loginDto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new UnauthorizedAccessException();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException();

            user.LastLoginAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return await CreateAuthResponseAsync(user);
        }



        public async Task LogoutAsync(RefreshTokenRequestDto requestDto)
        {
            ArgumentNullException.ThrowIfNull(requestDto);

            if (string.IsNullOrWhiteSpace(requestDto.RefreshToken))
                return;

            var refreshTokenHash = RefreshTokenHelper.Hash(requestDto.RefreshToken);

            var refreshToken = await _context.RefreshTokens
                .SingleOrDefaultAsync(refreshToken =>
                    refreshToken.TokenHash == refreshTokenHash);

            if (refreshToken is null || refreshToken.RevokedAt is not null)
                return;

            refreshToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


        public async Task<AuthResponseDto> RefreshAccessTokenAsync(
            RefreshTokenRequestDto requestDto)
        {
            ArgumentNullException.ThrowIfNull(requestDto);
            if (string.IsNullOrWhiteSpace(requestDto.RefreshToken))
                throw new UnauthorizedAccessException();
            
            var now = DateTime.UtcNow;
            var oldRefreshTokenHash = RefreshTokenHelper.Hash(requestDto.RefreshToken);
            
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var oldRefreshToken = await _context.RefreshTokens
                    .Include(refreshToken => refreshToken.User)
                    .SingleOrDefaultAsync(refreshToken =>
                        refreshToken.TokenHash == oldRefreshTokenHash);

                if (oldRefreshToken is null ||
                    !RefreshTokenHelper.IsActive(oldRefreshToken, now))
                {
                    throw new UnauthorizedAccessException();
                }
                oldRefreshToken.RevokedAt = now;

                var newRefreshTokenString = RefreshTokenHelper.Generate();
                var newRefreshTokenHash = RefreshTokenHelper.Hash(newRefreshTokenString);
                var newRefreshToken = 
                        RefreshTokenHelper.CreateEntity(
                                oldRefreshToken.UserId,
                                newRefreshTokenHash,
                                oldRefreshToken.ExpiresAt
                        );

                _context.RefreshTokens.Add(newRefreshToken);
                var newAccessToken = JwtHelper.GenerateToken(oldRefreshToken.User, _config);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return CreateAuthResponse(
                    oldRefreshToken.User,
                    newAccessToken,
                    newRefreshTokenString);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }



        private User CreateUser(string email, string password, string role)
        {
            // Guid.Empty უბრალოდ placeholder-ია სანამ DB ჩაწერს ნამდვილ GUID-ს.
            var user = new User
            {
                Email = email.Trim().ToLowerInvariant(), // User@Mail.com == user@mail.com
                Role = role.Trim()
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            return user;
        }


        private async Task<AuthResponseDto> CreateAuthResponseAsync(User user)
        {
            var accessToken = JwtHelper.GenerateToken(user, _config);

            var refreshTokenString = RefreshTokenHelper.Generate();
            var refreshTokenHash = RefreshTokenHelper.Hash(refreshTokenString);
            var refreshToken = 
                    RefreshTokenHelper.CreateEntity(
                            user.UserId, 
                            refreshTokenHash,
                            DateTime.UtcNow.AddDays(7)
                    );

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return CreateAuthResponse(user, accessToken, refreshTokenString);
        }


        private AuthResponseDto CreateAuthResponse(
            User user,
            string accessToken,
            string refreshToken)
        {
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                Role = user.Role
            };
        }
    }
}