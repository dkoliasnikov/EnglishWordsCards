using Domain.Abstraction;
using Domain.Models;
using Newtonsoft.Json;

namespace Domain.Services;

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

        if(_progress.ProgressMap.TryGetValue(literal, out int value))
        {
            return value;
        }

        _progress.ProgressMap[literal] = 0;
        return 0;
    }

    private void EnsureStorageLoaded()
    {
        if(_progress == null)
        {
            EnsureStorageFileCreated();
            _progress = JsonConvert.DeserializeObject<ShuffleCardsProgress>(File.ReadAllText(_storagePath));
        }
    }

    private void EnsureStorageFileCreated()
    {
        if (!File.Exists(_storagePath))
        {
            File.WriteAllText(_storagePath, JsonConvert.SerializeObject(new ShuffleCardsProgress(), Formatting.Indented));
        }
    }

    private void SaveProgressStorage()
    {
        File.WriteAllText(_storagePath, JsonConvert.SerializeObject(_progress, Formatting.Indented));
    }

    public void Increment(string literal)
    {
        if (Get(literal) == null)
            _progress.ProgressMap[literal] = 0;

        _progress.ProgressMap[literal] = _progress.ProgressMap[literal] + 1;
        SaveProgressStorage();
    }

    public void Decrement(string literal)
    {
        if (Get(literal) == null)
            _progress.ProgressMap[literal] = 0;

        _progress.ProgressMap[literal]--;
        SaveProgressStorage();
    }
}
