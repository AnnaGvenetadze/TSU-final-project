namespace VolunteerMatch.Validators
{
    public class UrlValidator
    {
        public static void ValidateLinkedInUrl(string? linkedInUrl)
        {
            if (linkedInUrl != null && !linkedInUrl.Contains("linkedin.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("LinkedIn ბმული უნდა ეკუთვნოდეს linkedin.com დომენს.");
            }
        }
    }
}
