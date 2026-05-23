namespace VolunteerMatch.Domain.Constants
{
    public enum MatchStatus : byte
    {
        Rejected = 0,
        Accepted = 1,
        
        Recommended = 2, // AI-მ ურჩია, მაგრამ მოხალისეს/ორგანიზაციას ჯერ არ გაუგზავნია მოთხოვნა
        Pending = 3, // მოთხოვნა გაგზავნილია მეორე მხარესთან და პასუხს ელოდება
        Expired = 4
    }
}

