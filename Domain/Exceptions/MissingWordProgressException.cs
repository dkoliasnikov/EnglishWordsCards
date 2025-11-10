namespace Domain.Exceptions;

internal class MissingWordProgressException : Exception
{
    public MissingWordProgressException(string word)
        : base($"Missing word progress. Word \"{word}\"")
    {
    }
}
