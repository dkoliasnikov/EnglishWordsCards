namespace Domain.Exceptions;

internal class NonUniqueLiteralException : Exception
{
    public NonUniqueLiteralException(IEnumerable<string> failedLiterals)
        : base($"Adding already existing literals in vocabulary: {string.Join(", ", failedLiterals)}")
    {
    }
}
