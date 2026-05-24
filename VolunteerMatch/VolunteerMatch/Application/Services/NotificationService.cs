//// TODO: ნოთიფიკაციას ქმნი მეჩინგის სერვისში

////var notification = _notificationFactory.CreateMatchProposedByOrganization(
////    volunteerId,
////eventEntity,
////organizationName);

////await _notificationService.CreateAsync(notification);
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using VolunteerMatch.Application.Dtos;
//using VolunteerMatch.Application.Interfaces;
//using VolunteerMatch.Domain.Models;
//using VolunteerMatch.Infrastructure.Data;


//namespace VolunteerMatch.Application.Services
//{
//    public class NotificationService : INotificationService
//    {
//        private readonly VolunteerMatchingDbContext _context;
//        private readonly IMapper _mapper;

//        public NotificationService(
//            VolunteerMatchingDbContext context,
//            IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        public async Task CreateNotificationAsync(Notification notification)
//        {
//            ArgumentNullException.ThrowIfNull(notification);

//            _context.Notifications.Add(notification);
//            await _context.SaveChangesAsync();
//        }

//        public async Task<List<GetNotificationDto>> GetMyNotificationsAsync(Guid userId)
//        {
//            await RemoveExpiredNotificationsAsync();

//            var notifications = await _context.Notifications
//                .AsNoTracking()
//                .Include(n => n.Event)
//                    .ThenInclude(e => e.Organization)
//                .Include(n => n.Event)
//                    .ThenInclude(e => e.EventTags)
//                        .ThenInclude(et => et.Tag)
//                .Where(n => n.UserId == userId && n.Event.IsActive)
//                .OrderByDescending(n => n.CreatedAt)
//                .ToListAsync();

//            return _mapper.Map<List<GetNotificationDto>>(notifications);
//        }

//        public async Task RemoveExpiredNotificationsAsync()
//        {
//            var now = DateTimeOffset.UtcNow;

//            var expiredNotifications = await _context.Notifications
//                .Where(n => n.ExpiresAt <= now)
//                .ToListAsync();

//            if (expiredNotifications.Count == 0)
//                return;

//            _context.Notifications.RemoveRange(expiredNotifications);
//            await _context.SaveChangesAsync();
//        }
//    }
//}