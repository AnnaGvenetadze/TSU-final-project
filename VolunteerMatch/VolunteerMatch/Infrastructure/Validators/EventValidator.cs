using Microsoft.AspNetCore.Http.HttpResults;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Infrastructure.Validators
{
    public static class EventValidator
    {
        public static void ValidateForCreate(CreateEventDetailsDto createDto)
        {
            if (createDto == null) 
                throw new ArgumentNullException(nameof(createDto), "DTO არ შეიძლება იყოს null");

            ValidateTextFields(
                createDto.Title,
                createDto.Description,
                createDto.Requirements,
                createDto.Location,
                createDto.Benefits);

            ValidateDateRange(createDto.StartDate, createDto.EndDate);
            ValidateDailyTimeRange(createDto.DailyStartTime, createDto.DailyEndTime);
        }


        public static void ValidateForUpdate(UpdateEventDetailsDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto), "DTO არ შეიძლება იყოს null");


            ValidateTextFields(
                updateDto.Title,
                updateDto.Description,
                updateDto.Requirements,
                updateDto.Location,
                updateDto.Benefits);

            ValidateDateRange(updateDto.StartDate, updateDto.EndDate);
            ValidateDailyTimeRange(updateDto.DailyStartTime, updateDto.DailyEndTime);
        }


        private static void ValidateTextFields(
            string title,
            string description,
            string requirements,
            string location,
            string benefits)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("სათაური სავალდებულოა.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("აღწერა სავალდებულოა.");

            if (string.IsNullOrWhiteSpace(requirements))
                throw new ArgumentException("მოთხოვნები სავალდებულოა.");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("ლოკაცია სავალდებულოა.");

            if (string.IsNullOrWhiteSpace(benefits))
                throw new ArgumentException("ბენეფიტები სავალდებულოა.");
        }


        public static void TrimEntityTextFields(Event eventEntity)
        {
            eventEntity.Title = eventEntity.Title.Trim();
            eventEntity.Description = eventEntity.Description.Trim();
            eventEntity.Requirements = eventEntity.Requirements.Trim();
            eventEntity.Location = eventEntity.Location.Trim();
            eventEntity.Benefits = eventEntity.Benefits.Trim();

            eventEntity.MainPhotoUrl = eventEntity.MainPhotoUrl?.Trim();
            eventEntity.Photo2Url = eventEntity.Photo2Url?.Trim();
            eventEntity.Photo3Url = eventEntity.Photo3Url?.Trim();
            eventEntity.AdditionalInfo = eventEntity.AdditionalInfo?.Trim();
        }

        private static void ValidateDateRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("დასრულების თარიღი არ შეიძლება იყოს დაწყების თარიღზე ადრე.");
        }


        private static void ValidateDailyTimeRange(TimeOnly dailyStartTime, TimeOnly dailyEndTime)
        {
            if (dailyEndTime <= dailyStartTime)
                throw new ArgumentException("დღის დასრულების დრო უნდა იყოს დაწყების დროზე გვიან.");
        }
    }
}