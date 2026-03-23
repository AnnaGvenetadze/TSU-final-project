using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class EventTagService : IEventTagService
    {
        private readonly VolunteerMatchingDbContext _context;

        public EventTagService(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task SaveEventTagsAsync(Guid eventId, List<Guid> tagIds)
        {
            var distinctTagIds = tagIds.Distinct().ToList();
            var eventTags = new List<EventTag>();

            for (int i = 0; i < distinctTagIds.Count; i++)
            {
                eventTags.Add(new EventTag
                {
                    EventId = eventId,
                    TagId = distinctTagIds[i],
                    SortOrder = i + 1
                });
            }

            if (eventTags.Count > 0)
                _context.EventTags.AddRange(eventTags);

            await _context.SaveChangesAsync();
        }


        public async Task SyncEventTagsAsync(Guid eventId, List<Guid> selectedTagIds)
        {
            var currentEventTags = await GetCurrentEventTagsAsync(eventId);

            var eventTagsToAdd = BuildEventTagsToAdd(eventId, selectedTagIds, currentEventTags);
            var eventTagsToRemove = BuildEventTagsToRemove(selectedTagIds, currentEventTags);

            UpdateSortOrders(selectedTagIds, currentEventTags);

            if (eventTagsToAdd.Count > 0)
                _context.EventTags.AddRange(eventTagsToAdd);

            if (eventTagsToRemove.Count > 0)
                _context.EventTags.RemoveRange(eventTagsToRemove);

            await _context.SaveChangesAsync();
        }


        private async Task<List<EventTag>> GetCurrentEventTagsAsync(Guid eventId)
        {
            return await _context.EventTags
                .Where(et => et.EventId == eventId)
                .ToListAsync();
        }


        private List<EventTag> BuildEventTagsToAdd(
            Guid eventId,
            List<Guid> selectedTagIds,
            List<EventTag> currentEventTags)
        {
            var currentTagIds = currentEventTags
                .Select(et => et.TagId)
                .ToHashSet();

            var eventTagsToAdd = new List<EventTag>();

            for (int i = 0; i < selectedTagIds.Count; i++)
            {
                var selectedTagId = selectedTagIds[i];

                if (!currentTagIds.Contains(selectedTagId))
                {
                    eventTagsToAdd.Add(new EventTag
                    {
                        EventId = eventId,
                        TagId = selectedTagId,
                        SortOrder = i + 1
                    });
                }
            }

            return eventTagsToAdd;
        }


        private List<EventTag> BuildEventTagsToRemove(
            List<Guid> selectedTagIds,
            List<EventTag> currentEventTags)
        {
            var selectedTagIdsSet = selectedTagIds.ToHashSet();
            var eventTagsToRemove = new List<EventTag>();

            foreach (var eventTag in currentEventTags)
            {
                if (!selectedTagIdsSet.Contains(eventTag.TagId))
                {
                    eventTagsToRemove.Add(eventTag);
                }
            }

            return eventTagsToRemove;
        }


        private void UpdateSortOrders(
            List<Guid> selectedTagIds,
            List<EventTag> currentEventTags)
        {
            for (int i = 0; i < selectedTagIds.Count; i++)
            {
                var selectedTagId = selectedTagIds[i];

                var existingEventTag = currentEventTags
                    .FirstOrDefault(et => et.TagId == selectedTagId);

                if (existingEventTag != null)
                {
                    existingEventTag.SortOrder = i + 1;
                }
            }
        }
    }
}