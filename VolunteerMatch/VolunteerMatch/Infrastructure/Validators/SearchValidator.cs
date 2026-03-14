namespace VolunteerMatch.Infrastructure.Validators
{
    public static class SearchValidator
    {
        public static string ValidateAndNormalize(string searchTerm, int take)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Trim().Length < 2)
                throw new ArgumentException("ძებნის ველი უნდა შეიცავდეს მინიმუმ 2 სიმბოლოს.");

            if (take <= 0 || take > 80)
                throw new ArgumentException("მისაცემი სია დიდია, შეამცირე მოთხოვნის რაოდენობა.");

            return searchTerm.Trim().ToLowerInvariant();
        }
    }
}
