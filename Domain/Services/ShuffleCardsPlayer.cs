using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;

namespace Domain.Services;

internal class ShuffleCardsPlayer : IShuffleCardsPlayer
{
    private readonly IVocabularyStorage _vocbularyStorage;
    private readonly IMainLog _log;

    public ShuffleCardsPlayer(IVocabularyStorage vocbularyStorage, IMainLog log)
    {
        _vocbularyStorage = vocbularyStorage;
        _log = log;
    }

    public async Task Play()
    {
        var vocalublar = await _vocbularyStorage.GetVocabularyAsync();

        var words = vocalublar.Words.Shuffle();

        foreach (var word in words)
        {
            Console.Clear();
            _log.AppendLine($"{word.Literal}{new string('\n',20)}{word.Translation}");
            Console.ReadKey();
        }
    }
}
