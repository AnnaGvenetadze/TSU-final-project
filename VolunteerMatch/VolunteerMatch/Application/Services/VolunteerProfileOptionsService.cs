using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class VolunteerProfileOptionsService : IVolunteerProfileOptionsService
    {
        private readonly VolunteerMatchingDbContext _context;

        public VolunteerProfileOptionsService(VolunteerMatchingDbContext context)
        {
            _context = context;
        }

        public async Task<GetVolunteerProfileOptionsDto> GetVolunteerProfileOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .OrderBy(skill => skill.Name)
                .Select(skill => new SelectOptionDto
                {
                    Id = skill.SkillId,
                    Name = skill.Name
                })
                .ToListAsync(cancellationToken);

            var interests = await _context.Interests
                .AsNoTracking()
                .OrderBy(interest => interest.Name)
                .Select(interest => new SelectOptionDto
                {
                    Id = interest.InterestId,
                    Name = interest.Name
                })
                .ToListAsync(cancellationToken);

            return new GetVolunteerProfileOptionsDto
            {
                Skills = skills,
                Interests = interests
            };
        }
    }
}