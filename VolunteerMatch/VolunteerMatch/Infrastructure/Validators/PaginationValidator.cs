namespace VolunteerMatch.Infrastructure.Validators
{
    public static class PaginationValidator
    {
        public static void Validate(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("გვერდის ნომერი უნდა იყოს მინიმუმ 1.");

            if (pageSize < 1)
                throw new ArgumentException("გვერდის ზომა უნდა იყოს მინიმუმ 1.");

            if (pageSize > 50)
                throw new ArgumentException("გვერდის ზომა არ უნდა აღემატებოდეს 50-ს.");
        }
    }
}