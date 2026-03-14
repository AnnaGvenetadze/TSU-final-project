using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;


namespace VolunteerMatch.Application.Services
{
    public class UserService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(
            VolunteerMatchingDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration config,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }


        public async Task<AuthResponseDto> RegisterVolunteerAsync(CreateVolunteerDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);
            VolunteerProfileValidator.ValidateForCreate(createDto);

            var user = CreateUser(createDto.Email, createDto.Password, "მოხალისე");
            var profile = _mapper.Map<VolunteerProfile>(createDto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.VolunteerId = user.UserId;
                _context.VolunteerProfiles.Add(profile);

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return CreateAuthResponse(user);
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
        }


        public async Task<AuthResponseDto> RegisterOrganizationAsync(CreateOrganizationDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var user = CreateUser(createDto.Email, createDto.Password, "ორგანიზაცია");
            var profile = _mapper.Map<OrganizationProfile>(createDto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.OrganizationId = user.UserId;
                _context.OrganizationProfiles.Add(profile);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return CreateAuthResponse(user);
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
        }


        public async Task<AuthResponseDto> AuthenticateUserAsync(LoginUserDto loginDto)
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

            return CreateAuthResponse(user);
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


        private AuthResponseDto CreateAuthResponse(User user)
        {
            var token = JwtHelper.GenerateToken(user, _config);

            return new AuthResponseDto
            {
                AccessToken = token,
                UserId = user.UserId,
                Role = user.Role,
            };
        }
    }
}