namespace VolunteerMatch.Exceptions;

public sealed class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
    : base("იმეილი უკვე დაკავებულია. გთხოვთ, ჩაწეროთ სხვა.")
    {
    }
}
