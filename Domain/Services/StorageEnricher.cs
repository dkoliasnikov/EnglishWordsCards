using Domain.Abstraction;
using Domain.Models;
using System.Text.RegularExpressions;

namespace Domain.Services;

internal partial class StorageEnricher : IStorageEnricher
{
    [GeneratedRegex("\\s")]
    private static partial Regex SingleSpaceRegex();

    private readonly Options _options;
    private readonly IVocabularyStorage _vocabularyStorage;

    public StorageEnricher(Options options, IVocabularyStorage vocabularyStorage)
    {
        _options = options;
        _vocabularyStorage = vocabularyStorage;
    }

    public async Task EnrichAsync()
    {
        ThrowOnSeedFileNotExists();
        var words = File.ReadAllLines(_options.WordsSeedPath)
           .Select(row =>
           {
               var tokens = SingleSpaceRegex().Split(row);
               if (tokens.Length != 3)
               {
                   tokens[2] = string.Join(" ", tokens.Take(2..));
               }

               return new Word(tokens[0], tokens[1], tokens[2]);
           })
           .ToList();

        await _vocabularyStorage.AddNewWordsAsync(words);
    }

    private void ThrowOnSeedFileNotExists()
    {
        if (!File.Exists(_options.WordsSeedPath))
            throw new FileNotFoundException("Words seed file does not exists", _options.WordsSeedPath);
    }
}
