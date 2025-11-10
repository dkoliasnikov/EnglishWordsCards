using Domain.Abstraction;
using Domain.Exceptions;
using Domain.Models;
using Domain.Serialization;
using Newtonsoft.Json;

namespace Domain.Services.Storage;

internal class VocabularyStorage : IVocabularyStorage
{
    private readonly Options _options;

    public VocabularyStorage(Options options)
    {
        _options = options;
    }

    public async Task AddNewWordsAsync(IEnumerable<Word> words)
    {
        EnsureStorageFileCreated();
        var vocabulary = await GetVocabularyAsync();
        var existedLiterals = vocabulary!.Words!.Select(word => word.Literal.ToLower());

        var intersected = existedLiterals.Intersect(words.Select(word => word.Literal.ToLower()));

        if (intersected.Any())
        {
            throw new NonUniqueLiteralException(intersected);
        }

        vocabulary.Words.AddRange(words);
        File.WriteAllText(_options.VocabularyStoragePath, JsonConvert.SerializeObject(vocabulary, Formatting.Indented, DefaultSettings.DefaultJsonSerializationSettings));
    }

    public Task<Vocabulary> GetVocabularyAsync()
    {
        ThrowOnStorageFileNotExists();
        var vocabulary = JsonConvert.DeserializeObject<Vocabulary>(File.ReadAllText(_options.VocabularyStoragePath), DefaultSettings.DefaultJsonDeserializationSettings);

        return Task.FromResult(new Vocabulary(vocabulary.Words.Where(w => !string.IsNullOrEmpty(w.Literal)).ToList()));
    }

    private void EnsureStorageFileCreated()
    {
        if (!File.Exists(_options.VocabularyStoragePath))
        {
            File.WriteAllText(_options.VocabularyStoragePath, JsonConvert.SerializeObject(new Vocabulary(new()), Formatting.Indented, DefaultSettings.DefaultJsonSerializationSettings));
        }
    }

    private void ThrowOnStorageFileNotExists()
    {
        if (!File.Exists(_options.VocabularyStoragePath))
            throw new FileNotFoundException("Vocabulary storage file does not exists", _options.VocabularyStoragePath);
    }
}
