using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services;

internal class LeastKnownCards : ILeastKnown
{
    private readonly IVocabularyStorage _vocabularyStorage;
    private readonly IMainLog _log;
    private readonly IShuffleCardsProgressStorage _shuffleCardsProgressStorage;
    private readonly Options _options;
    private readonly int _delay;

    public LeastKnownCards(IVocabularyStorage vocbularyStorage, IMainLog log, IShuffleCardsProgressStorage shuffleCardsProgressStorage, Options options)
    {
        _vocabularyStorage = vocbularyStorage;
        _log = log;
        _shuffleCardsProgressStorage = shuffleCardsProgressStorage;
        _options = options;
        _delay = _options.DelayBeforeNextCard;
    }

    public async Task Play()
    {
        var vocabular = await _vocabularyStorage.GetVocabularyAsync();

        var words = vocabular
            .Words
            .Select(w => new { Card = w, Progress = _shuffleCardsProgressStorage.Get(w.Literal)})
            .OrderBy(w => w.Progress);

        foreach (var word in words)
        {
            Console.Clear();
            _log.AppendLine($"{word.Card.Literal}");

            while (true) 
            {
                var key = Console.ReadKey();
                ClearReadKey();

                if (key.KeyChar == '0')
                    ShowTranslation(word.Card);
                else if (key.KeyChar == '+')
                {
                    Increment(word.Card.Literal);
                    ShowTranslation(word.Card);
                    await Task.Delay(_delay);
                    break;
                }
                else if (key.KeyChar == '-')
                {
                    Decrement(word.Card.Literal);
                    ShowTranslation(word.Card);
                    await Task.Delay(_delay);
                    break;
                }
                else
                {
                    break;
                }
            }

        }
    }

    public string? GetShortcuts() => $"'0' - show translation. '+' - i know the answer. '-' - i don`t know the answer";

    void ClearReadKey()
    {
        if (Console.CursorLeft <= 0)
            return;

        Console.CursorLeft -= 1;
        Console.Write(" ");
        Console.CursorLeft -= 1;
    }

    private void Decrement(string literal) => _shuffleCardsProgressStorage.Decrement(literal);

    private void Increment(string literal) => _shuffleCardsProgressStorage.Increment(literal);

    private void ShowTranslation(Word card) => _log.AppendLine($"{new string('\n', 5)}[{_shuffleCardsProgressStorage.Get(card.Literal)}] {card.Translation}");
}
