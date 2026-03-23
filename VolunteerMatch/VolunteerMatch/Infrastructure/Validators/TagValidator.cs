using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Infrastructure.Validators
{
    public class TagValidator : ITagValidator
    {
        private readonly VolunteerMatchingDbContext _context;

        public TagValidator(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ValidateSelectedTagIdsAsync(List<Guid> selectedTagIds)
        {
            if (selectedTagIds == null || selectedTagIds.Count == 0)
                throw new ArgumentException("მინიმუმ ერთი თემატიკის არჩევა აუცილებელია.");

            var distinctTagIds = selectedTagIds.Distinct().ToList();

            var existingTagIds = await _context.Tags
                .Where(t => distinctTagIds.Contains(t.TagId))
                .Select(t => t.TagId)
                .ToListAsync();

            if (existingTagIds.Count != distinctTagIds.Count)
                throw new ArgumentException("არჩეული თემატიკებიდან ერთი ან რამდენიმე არასწორია.");
        }
    }
}
