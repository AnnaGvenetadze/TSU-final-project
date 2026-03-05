using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Exceptions;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Models;

namespace VolunteerMatch.Services
{
    public class UserService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;

        public UserService(
            VolunteerMatchingDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public async Task RegisterVolunteerAsync(RegisterVolunteerDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = CreateUser(dto.Email, dto.Password, "მოხალისე");
            var profile = CreateVolunteerProfile(dto, user);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.VolunteerId = user.UserId;
                _context.VolunteerProfiles.Add(profile);

                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch (DbUpdateException ex)
                when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                await tx.RollbackAsync();
                throw new DuplicateEmailException();
            }
        }


        public async Task RegisterOrganizationAsync(RegisterOrganizationDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = CreateUser(dto.Email, dto.Password, "ორგანიზაცია");
            var organization = CreateOrganizationProfile(dto, user);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var profile = CreateOrganizationProfile(dto, user);

                _context.OrganizationProfiles.Add(profile);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateException ex)
                when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                await tx.RollbackAsync();
                throw new DuplicateEmailException();
            }
        }


        public async Task<string> AuthenticateUserAsync(LoginUserDto dto)
        {
            // DTO already validated by [ApiController] -> ModelState
            ArgumentNullException.ThrowIfNull(dto);

            var email = dto.Email.Trim();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new UnauthorizedAccessException();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException();

            user.LastLoginAt = DateTimeOffset.UtcNow; // DATETIMEOFFSET column
            await _context.SaveChangesAsync();

            return JwtHelper.GenerateToken(user, _config);
        }


        private User CreateUser(string email, string password, string role)
        {
            // Guid.Empty უბრალოდ placeholder-ია სანამ DB ჩაწერს ნამდვილ GUID-ს.
            var user = new User
            {
                Email = email.Trim(),
                Role = role,
                LastLoginAt = null
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            return user;
        }


        private VolunteerProfile CreateVolunteerProfile(RegisterVolunteerDto dto, User user)
        {
            return new VolunteerProfile
            {
                VolunteerId = user.UserId,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                BirthDate = dto.BirthDate,
                Citizenship = dto.Citizenship.Trim(),
                Profession = dto.Profession.Trim(),
                Languages = dto.Languages.Trim(),
                Skills = dto.Skills,
                Interests = dto.Interests
            };
        }


        private OrganizationProfile CreateOrganizationProfile(RegisterOrganizationDto dto, User user)
        {
            return new OrganizationProfile
            {
                OrganizationId = user.UserId,
                OrganizationName = dto.OrganizationName.Trim(),
                Description = dto.Description
            };
        }
    }
}
