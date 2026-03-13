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


        public async Task RegisterVolunteerAsync(CreateVolunteerDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);
            if (createDto.BirthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("დაბადების თარიღი უნდა იყოს წარსულში.");

            var user = CreateUser(createDto.Email, createDto.Password, "მოხალისე");
            var profile = CreateVolunteer(createDto);

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


        public async Task RegisterOrganizationAsync(CreateOrganizationDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var user = CreateUser(createDto.Email, createDto.Password, "ორგანიზაცია");
            var profile = CreateOrganization(createDto);

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


        public async Task<string> AuthenticateUserAsync(LoginUserDto loginDto)
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


        private VolunteerProfile CreateVolunteer(CreateVolunteerDto createDto)
        {
            return new VolunteerProfile
            {
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                BirthDate = createDto.BirthDate,
                Citizenship = createDto.Citizenship.Trim(),
                Profession = createDto.Profession.Trim(),
                Languages = createDto.Languages.Trim(),
                Skills = createDto.Skills.Trim(),
                Interests = createDto.Interests.Trim()
                // TODO: TagIds ლისტი დააბრუნე
            };
        }


        private OrganizationProfile CreateOrganization(CreateOrganizationDto createDto)
        {
            return new OrganizationProfile
            {
                OrganizationName = createDto.OrganizationName.Trim(),
                Description = createDto.Description.Trim()
            };
        }
    }
}