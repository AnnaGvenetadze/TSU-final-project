namespace VolunteerMatch.Application.Exceptions;

public sealed class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
    : base("იმეილი უკვე დაკავებულია. გთხოვთ, ჩაწეროთ სხვა.")
    {
    }
}
