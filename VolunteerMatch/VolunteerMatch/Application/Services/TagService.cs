using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class TagService
    {
        private readonly VolunteerMatchingDbContext _context;

        public TagService(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TagListItemDto>> GetAllAsync()
        {
            return await _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => new TagListItemDto
                {
                    TagId = t.TagId,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}