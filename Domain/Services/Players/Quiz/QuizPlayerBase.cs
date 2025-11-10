using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players.Quiz;

internal abstract class QuizPlayerBase : IQuizPlayer
{
    public virtual string Name { get => throw new NotImplementedException(); }
    protected readonly IVocabularyStorage VocabularyStorage;

    private readonly IMainLog _log;
    private readonly int _delayBeforeNextCard;

    public QuizPlayerBase(IVocabularyStorage vocabularyStorage, IMainLog log, Options options)
    {
        _log = log;
        VocabularyStorage = vocabularyStorage;
    }

    public string? GetShortcuts() => "1,2,3,4 - answer options";

    public async Task PlayAsync()
    {
        ConsoleDefaultColor();

        var words = await GetWordsAsync();

        foreach (var word in words)
        {
            await PlayWord(word, words);
            Console.Clear();
        }
    }

    protected abstract Task<ICollection<Word>> GetWordsAsync();

    private async Task PlayWord(Word word, IEnumerable<Word> words)
    {
        var roundWordsPool = words.Shuffle()
            .Take(4)
            .Where(x => !x.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToList();

        roundWordsPool.Add(word);
        roundWordsPool = roundWordsPool.Shuffle().ToList();

        _log.AppendLine($"{word.Literal}  [{word.Pronounce}]\n\n\n\n\n\n\n\n");

        var id = 0;
        int? correctAnswerId = null;
        foreach (var option in roundWordsPool)
        {
            if (option.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            {
                correctAnswerId = id;
                break;
            }
            id++;
        }

        var firstColumnWidth = Math.Max(roundWordsPool[0].Translation.Length, roundWordsPool[2].Translation.Length);

        _log.AppendLine($"1. {roundWordsPool[0].Translation.PadRight(firstColumnWidth)}   2. {roundWordsPool[1].Translation}");
        _log.AppendLine($"3. {roundWordsPool[2].Translation.PadRight(firstColumnWidth)}   4. {roundWordsPool[3].Translation}");

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
            await Task.Delay(_delayBeforeNextCard);
        }
        else
        {
            ConsoleIncorrectAnswerColor();
            _log.Append($"Incorrect!");
            ConsoleDefaultColor();
            _log.AppendLine($" Correct answer is {correctAnswerId + 1}. {word.Translation}");
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