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
            var profile = CreateVolunteerProfile(dto);

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
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
        }


        public async Task RegisterOrganizationAsync(RegisterOrganizationDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = CreateUser(dto.Email, dto.Password, "ორგანიზაცია");
            var profile = CreateOrganizationProfile(dto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.OrganizationId = user.UserId;
                _context.OrganizationProfiles.Add(profile);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();

                if (DbExceptionHelper.IsUniqueConstraintViolation(ex))
                    throw new DuplicateEmailException();

                throw; // 500
            }
        }


        public async Task<string> AuthenticateUserAsync(LoginUserDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new UnauthorizedAccessException();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException();

            user.LastLoginAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return JwtHelper.GenerateToken(user, _config);
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


        private VolunteerProfile CreateVolunteerProfile(RegisterVolunteerDto dto)
        {
            return new VolunteerProfile
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                BirthDate = dto.BirthDate,
                Citizenship = dto.Citizenship.Trim(),
                Profession = dto.Profession.Trim(),
                Languages = dto.Languages.Trim(),
                Skills = dto.Skills.Trim(),
                Interests = dto.Interests.Trim()
                // TODO: TagIds ლისტი დააბრუნე
            };
        }


        private OrganizationProfile CreateOrganizationProfile(RegisterOrganizationDto dto)
        {
            return new OrganizationProfile
            {
                OrganizationName = dto.OrganizationName.Trim(),
                Description = dto.Description.Trim()
            };
        }
    }
}
