using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Validators
{
    public static class EventValidator
    {
        public static void ValidateForCreate(CreateEventDetailsDto createDto)
        {
            ValidateDateAndTime(createDto.EndDate, createDto.StartDate,
                createDto.DailyEndTime, createDto.DailyStartTime);
            ValidateDateAndTimeNotInPast(createDto.StartDate, createDto.DailyStartTime);
        }


        public static void ValidateForUpdate(UpdateEventDetailsDto createDto)
        {
            ValidateDateAndTime(createDto.EndDate, createDto.StartDate,
                createDto.DailyEndTime, createDto.DailyStartTime);
            ValidateDateAndTimeNotInPast(createDto.StartDate, createDto.DailyStartTime);
        }


        private static void ValidateDateAndTime(
            DateTimeOffset endDate,
            DateTimeOffset startDate,
            TimeOnly dailyEndTime,
            TimeOnly dailyStartTime)
        {
            if (endDate < startDate)
                throw new ArgumentException("დასრულების თარიღი არ შეიძლება იყოს დაწყების თარიღზე ადრე.");
            if (dailyEndTime <= dailyStartTime)
                throw new ArgumentException("დღის დასრულების დრო უნდა იყოს დაწყების დროზე გვიან.");
        }


        private static void ValidateDateAndTimeNotInPast(DateTimeOffset startDate, TimeOnly dailyStartTime)
        {
            var now = DateTimeOffset.Now;
            var today = DateOnly.FromDateTime(now.DateTime);
            var currentTime = TimeOnly.FromDateTime(now.DateTime);
            var startDateOnly = DateOnly.FromDateTime(startDate.DateTime);

            if (startDateOnly < today)
                throw new ArgumentException("ივენთის დაწყების თარიღი არ შეიძლება იყოს წარსულში.");
            if (startDateOnly == today && dailyStartTime <= currentTime)
                throw new ArgumentException("დღევანდელი ივენთის დაწყების დრო უკვე გასულია.");
        }
    }
}