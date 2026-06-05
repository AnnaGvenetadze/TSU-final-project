using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;

namespace VolunteerMatch.Application.Services
{
    public class MyOrganizationMatchingService : IOrganizationMatchingService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly MatchCleanupHelper _matchCleanupHelper;

        public MyOrganizationMatchingService(
            VolunteerMatchingDbContext context,
            MatchCleanupHelper matchCleanupHelper)
        {
            _context = context;
            _matchCleanupHelper = matchCleanupHelper;
        }


        public async Task AcceptVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToVolunteerMatchRequestAsync(
                organizationId,
                matchId,
                MatchStatus.Accepted,
                cancellationToken);
        }



        public async Task DeclineVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToVolunteerMatchRequestAsync(
                organizationId,
                matchId,
                MatchStatus.Rejected,
                cancellationToken);
        }



        private async Task RespondToVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            MatchStatus newStatus,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .Include(match => match.Event)
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.Event.OrganizationId == organizationId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Pending ||
                match.RequestedByRole != UserRoles.Volunteer)
            {
                throw new ArgumentException(
                    "მოქმედება შესაძლებელია მხოლოდ მოხალისისგან შემოსულ მოთხოვნაზე.");
            }

            match.Status = newStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }




    }
}