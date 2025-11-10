using Domain.Abstraction;
using Domain.Exceptions;
using Domain.Models;
using Domain.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Domain.Services.Storage;

internal class ShuffleCardsProgressStorage : IShuffleCardsProgressStorage
{
    private readonly string _storagePath;
    private ShuffleCardsProgress _progress;

    public ShuffleCardsProgressStorage(Options options)
    {
        _storagePath = options.ShuffleCardsProgressStoragePath;
    }

    public void Put((string literal, int value) cardProgress)
    {
        _progress.ProgressMap[cardProgress.literal] = cardProgress.value;
        SaveProgressStorage();
    }

    public int Get(string literal)
    {
        EnsureStorageLoaded();

        if (_progress.ProgressMap.TryGetValue(literal, out int value))
        {
            return value;
        }

        throw new MissingWordProgressException(literal);
    }

    private void EnsureStorageLoaded()
    {
        if (_progress == null)
        {
            EnsureStorageFileCreated();
            _progress = JsonConvert.DeserializeObject<ShuffleCardsProgress>(File.ReadAllText(_storagePath), DefaultSettings.DefaultJsonDeserializationSettings);
        }
    }

    private void EnsureStorageFileCreated()
    {
        if (!File.Exists(_storagePath))
        {
            File.WriteAllText(_storagePath, JsonConvert.SerializeObject(new ShuffleCardsProgress(), Formatting.Indented, DefaultSettings.DefaultJsonSerializationSettings));
        }
    }

    private void SaveProgressStorage()
    {
        File.WriteAllText(_storagePath, JsonConvert.SerializeObject(_progress, Formatting.Indented, DefaultSettings.DefaultJsonSerializationSettings));
    }

    public void Increment(string literal)
    {
        _progress.ProgressMap[literal] += 1;
        SaveProgressStorage();
    }

    public void Decrement(string literal)
    {
        _progress.ProgressMap[literal]--;
        SaveProgressStorage();
    }

    public void Enrich(List<Word> words)
    {
        EnsureStorageLoaded();

        var newWords = words.Where(x => !_progress.ProgressMap.TryGetValue(x.Literal.ToLower(), out var _)).ToList();

        foreach (var word in newWords)
        {
            _progress.ProgressMap[word.Literal] = 0;
        }

        if (newWords.Any())
        {
            SaveProgressStorage();
        }
    }
}
