using Common.Log.Abstractions;
using Common.Extensions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services;

internal class QuizCardsPlayer : IQuizPlayer
{
    private readonly IVocabularyStorage _vocabularyStorage;
    private readonly IMainLog _log;
    private int _delay;

    public QuizCardsPlayer(IVocabularyStorage vocbularyStorage, IMainLog log, Options options)
    {
        _vocabularyStorage = vocbularyStorage;
        _log = log;
        _delay = options.DelayBeforeNextCard;
    }

    public async Task Play()
    {
        ConsoleDefaultColor();
        var vocabular = await _vocabularyStorage.GetVocabularyAsync();

        var words = vocabular.Words.Shuffle();

        foreach (var word in words)
        {
            await PlayWord(word, words);
            Console.Clear();
        }
    }

    private async Task PlayWord(Word word, IEnumerable<Word> words)
    {
        var options = words.Shuffle()
            .Take(4)
            .Where(x => !x.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToList();

        options.Add(word);
        options = options.Shuffle().ToList();
        _log.AppendLine($"{word.Literal}  [{word.Pronounce}]\n\n\n\n\n\n\n\n");

        var id = 0;
        int? correctAnswerId = null;
        foreach (var option in options)
        {
            if (option.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            {
                correctAnswerId = id;
                break;
            }
            id++;
        }
        var firstColumnWidth = Math.Max(options[0].Translation.Length, options[2].Translation.Length);

        _log.AppendLine($"1. {options[0].Translation.PadRight(firstColumnWidth)}   2. {options[1].Translation}");
        _log.AppendLine($"3. {options[2].Translation.PadRight(firstColumnWidth)}   4. {options[3].Translation}");

        int userAnswer;
        while (!int.TryParse(new string(Console.ReadKey().KeyChar, 1), out userAnswer))
        {
            _log.AppendLine("Failed parse answer");
        }

        _log.AppendLine(string.Empty);

        if (--userAnswer == correctAnswerId)
        {
            ConsoleCorrectAnswerColor();
            _log.AppendLine("Correct!");
            ConsoleDefaultColor();
            await Task.Delay(_delay);
        }
        else
        {
            ConsoleIncorrectAnswerColor();
            _log.Append($"Incorrect!");
            ConsoleDefaultColor();
            _log.AppendLine($" Correct answer is {correctAnswerId}. {word.Translation}");
            Console.ReadKey();
        }
    }

    private static void ConsoleIncorrectAnswerColor()
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void ConsoleCorrectAnswerColor()
    {
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void ConsoleDefaultColor()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }

}
