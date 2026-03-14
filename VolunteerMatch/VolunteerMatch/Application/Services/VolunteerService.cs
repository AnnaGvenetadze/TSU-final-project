using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Data;


namespace VolunteerMatch.Application.Services
{
    public class VolunteerService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public VolunteerService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetVolunteerProfileDto> GetVolunteerProfileAsync(Guid volunteerId)
        {
            var profile = await _context.VolunteerProfiles
                 .Include(v => v.Volunteer)
                 .SingleOrDefaultAsync(v => v.VolunteerId == volunteerId);

            if (profile == null)
            {
                throw new KeyNotFoundException("მოხალისის პროფილი ვერ მოიძებნა.");
            }

            return _mapper.Map<GetVolunteerProfileDto>(profile);
        }

        public async Task<List<SearchVolunteerItemDto>> SearchVolunteersAsync(string searchTerm, int take)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Trim().Length < 2)
                throw new ArgumentException("ძებნის ველი უნდა შეიცავდეს მინიმუმ 2 სიმბოლოს.");
            if (!(take > 0 && take <= 80))
                throw new ArgumentException("მისაცემი სია დიდია, შეამცირე მოთხოვნის რაოდენობა.");

            searchTerm = searchTerm.Trim().ToLower();

            var volunteers = await _context.VolunteerProfiles
                .Include(v => v.Volunteer)
                .Where(v =>
                    v.FirstName.ToLower().Contains(searchTerm) ||
                    v.LastName.ToLower().Contains(searchTerm) ||
                    (v.FirstName + " " + v.LastName).ToLower().Contains(searchTerm))
                .OrderBy(v => v.FirstName)
                .ThenBy(v => v.LastName)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<List<SearchVolunteerItemDto>>(volunteers);
        }
    }
}