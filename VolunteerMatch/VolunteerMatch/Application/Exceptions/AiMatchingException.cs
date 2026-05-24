namespace VolunteerMatch.Application.Exceptions
{
    public class AiMatchingException : Exception
    {
        public AiMatchingException(string message)
            : base(message)
        {
        }

        public AiMatchingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}