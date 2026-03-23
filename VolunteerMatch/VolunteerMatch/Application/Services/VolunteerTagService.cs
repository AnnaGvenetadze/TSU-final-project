using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Application.Services
{
    public class VolunteerTagService : IVolunteerTagService
    {
        private readonly VolunteerMatchingDbContext _context;

        public VolunteerTagService(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task SaveVolunteerTagsAsync(Guid volunteerId, List<Guid> tagIds)
        {
            var volunteerTags = new List<VolunteerTag>();

            foreach (var tagId in tagIds.Distinct())
            {
                volunteerTags.Add(new VolunteerTag
                {
                    VolunteerId = volunteerId,
                    TagId = tagId
                });
            }
            if (volunteerTags.Count > 0)
                _context.VolunteerTags.AddRange(volunteerTags);

            await _context.SaveChangesAsync();
        }


        public async Task SyncVolunteerTagsAsync(Guid volunteerId, List<Guid> selectedTagIds)
        {
            var currentVolunteerTags = await GetCurrentVolunteerTagsAsync(volunteerId);

            var volunteerTagsToAdd = BuildVolunteerTagsToAdd(volunteerId, selectedTagIds, currentVolunteerTags);
            var volunteerTagsToRemove = BuildVolunteerTagsToRemove(selectedTagIds, currentVolunteerTags);

            if (volunteerTagsToAdd.Count > 0)
                _context.VolunteerTags.AddRange(volunteerTagsToAdd);
            if (volunteerTagsToRemove.Count > 0)
                _context.VolunteerTags.RemoveRange(volunteerTagsToRemove);

            await _context.SaveChangesAsync();
        }


        private async Task<List<VolunteerTag>> GetCurrentVolunteerTagsAsync(Guid volunteerId)
        {
            return await _context.VolunteerTags
                .Where(vt => vt.VolunteerId == volunteerId)
                .ToListAsync();
        }


        private List<VolunteerTag> BuildVolunteerTagsToAdd(
            Guid volunteerId,
            List<Guid> selectedTagIds,
            List<VolunteerTag> currentVolunteerTags)
        {
            var currentTagIds = currentVolunteerTags
                .Select(vt => vt.TagId)
                .ToHashSet();

            var volunteerTagsToAdd = new List<VolunteerTag>();

            foreach (var selectedTagId in selectedTagIds)
            {
                if (!currentTagIds.Contains(selectedTagId))
                {
                    volunteerTagsToAdd.Add(new VolunteerTag
                    {
                        VolunteerId = volunteerId,
                        TagId = selectedTagId
                    });
                }
            }

            return volunteerTagsToAdd;
        }


        private List<VolunteerTag> BuildVolunteerTagsToRemove(
            List<Guid> selectedTagIds,
            List<VolunteerTag> currentVolunteerTags)
        {
            var volunteerTagsToRemove = new List<VolunteerTag>();

            foreach (var volunteerTag in currentVolunteerTags)
            {
                if (!selectedTagIds.Contains(volunteerTag.TagId))
                {
                    volunteerTagsToRemove.Add(volunteerTag);
                }
            }

            return volunteerTagsToRemove;
        }
    }
}