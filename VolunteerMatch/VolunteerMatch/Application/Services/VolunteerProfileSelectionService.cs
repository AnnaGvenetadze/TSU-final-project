using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class VolunteerProfileSelectionService : IVolunteerProfileSelectionService
    {
        private readonly VolunteerMatchingDbContext _context;

        public VolunteerProfileSelectionService(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task SaveVolunteerSkillsAndInterestsAsync(
            VolunteerProfile profile,
            List<Guid> selectedSkillIds,
            List<Guid> selectedInterestIds,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(profile);
            ArgumentNullException.ThrowIfNull(selectedSkillIds);
            ArgumentNullException.ThrowIfNull(selectedInterestIds);

            await SaveVolunteerSkillsAsync(profile, selectedSkillIds, cancellationToken);
            await SaveVolunteerInterestsAsync(profile, selectedInterestIds, cancellationToken);
        }



        public async Task SyncVolunteerSkillsAndInterestsAsync(
            VolunteerProfile profile,
            List<Guid> selectedSkillIds,
            List<Guid> selectedInterestIds,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(profile);
            ArgumentNullException.ThrowIfNull(selectedSkillIds);
            ArgumentNullException.ThrowIfNull(selectedInterestIds);

            await DeleteVolunteerSkillsAsync(profile.VolunteerId, cancellationToken);
            await DeleteVolunteerInterestsAsync(profile.VolunteerId, cancellationToken);

            await SaveVolunteerSkillsAndInterestsAsync(
                profile,
                selectedSkillIds,
                selectedInterestIds,
                cancellationToken);
        }



        private async Task SaveVolunteerSkillsAsync(
            VolunteerProfile profile,
            List<Guid> selectedSkillIds,
            CancellationToken cancellationToken = default)
        {
            var distinctSkillIds = selectedSkillIds.Distinct().ToList();

            if (!distinctSkillIds.Any())
            {
                profile.Skills = string.Empty;
                return;
            }

            var skills = await _context.Skills
                .Where(skill => distinctSkillIds.Contains(skill.SkillId))
                .OrderBy(skill => skill.Name)
                .ToListAsync(cancellationToken);

            if (skills.Count != distinctSkillIds.Count)
            {
                throw new ArgumentException("ერთი ან რამდენიმე არჩეული უნარი ვერ მოიძებნა.");
            }

            var volunteerSkills = skills
                .Select(skill => new VolunteerSkill
                {
                    VolunteerId = profile.VolunteerId,
                    SkillId = skill.SkillId
                })
                .ToList();

            await _context.VolunteerSkills.AddRangeAsync(volunteerSkills, cancellationToken);

            profile.Skills = string.Join(", ", skills.Select(skill => skill.Name));
        }



        private async Task SaveVolunteerInterestsAsync(
            VolunteerProfile profile,
            List<Guid> selectedInterestIds,
            CancellationToken cancellationToken = default)
        {
            var distinctInterestIds = selectedInterestIds.Distinct().ToList();

            if (!distinctInterestIds.Any())
            {
                profile.Interests = string.Empty;
                return;
            }

            var interests = await _context.Interests
                .Where(interest => distinctInterestIds.Contains(interest.InterestId))
                .OrderBy(interest => interest.Name)
                .ToListAsync(cancellationToken);

            if (interests.Count != distinctInterestIds.Count)
            {
                throw new ArgumentException("ერთი ან რამდენიმე არჩეული ინტერესი ვერ მოიძებნა.");
            }

            var volunteerInterests = interests
                .Select(interest => new VolunteerInterest
                {
                    VolunteerId = profile.VolunteerId,
                    InterestId = interest.InterestId
                })
                .ToList();

            await _context.VolunteerInterests.AddRangeAsync(volunteerInterests, cancellationToken);

            profile.Interests = string.Join(", ", interests.Select(interest => interest.Name));
        }



        private async Task DeleteVolunteerSkillsAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            var oldSkills = await _context.VolunteerSkills
                .Where(volunteerSkill => volunteerSkill.VolunteerId == volunteerId)
                .ToListAsync(cancellationToken);

            _context.VolunteerSkills.RemoveRange(oldSkills);
        }



        private async Task DeleteVolunteerInterestsAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            var oldInterests = await _context.VolunteerInterests
                .Where(volunteerInterest => volunteerInterest.VolunteerId == volunteerId)
                .ToListAsync(cancellationToken);

            _context.VolunteerInterests.RemoveRange(oldInterests);
        }
    }
}