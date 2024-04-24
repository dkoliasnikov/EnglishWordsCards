using Common.Log.Abstractions;
using Common.Extensions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services;

internal class QuizCardsPlayer : IQuizPlayer
{
	public string Name { get => "Quiz"; }

	private readonly IVocabularyStorage _vocabularyStorage;
    private readonly IMainLog _log;
    private int _delayBeforeNextCard;

    public QuizCardsPlayer(IVocabularyStorage vocbularyStorage, IMainLog log, Options options)
    {
        _vocabularyStorage = vocbularyStorage;
        _log = log;
        _delayBeforeNextCard = options.DelayBeforeNextCard;
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
        var wordsPool = words.Shuffle()
            .Take(4)
            .Where(x => !x.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToList();

        wordsPool.Add(word);
        wordsPool = wordsPool.Shuffle().ToList();
        _log.AppendLine($"{word.Literal}  [{word.Pronounce}]\n\n\n\n\n\n\n\n");

        var id = 0;
        int? correctAnswerId = null;
        foreach (var option in wordsPool)
        {
            if (option.Literal.Equals(word.Literal, StringComparison.OrdinalIgnoreCase))
            {
                correctAnswerId = id;
                break;
            }
            id++;
        }
        var firstColumnWidth = Math.Max(wordsPool[0].Translation.Length, wordsPool[2].Translation.Length);

        _log.AppendLine($"1. {wordsPool[0].Translation.PadRight(firstColumnWidth)}   2. {wordsPool[1].Translation}");
        _log.AppendLine($"3. {wordsPool[2].Translation.PadRight(firstColumnWidth)}   4. {wordsPool[3].Translation}");

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
            _log.AppendLine($" Correct answer is {correctAnswerId}. {word.Translation}");
            Console.ReadKey();
        }
    }

    public string? GetShortcuts() => "1,2,3,4 - answer options";

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