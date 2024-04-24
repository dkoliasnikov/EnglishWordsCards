using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services;

internal abstract class ProgressCardsPlayerBase : ICardsPlayer
{
	protected readonly IVocabularyStorage _vocabularyStorage;
	protected readonly IMainLog _log;
	protected readonly IShuffleCardsProgressStorage _shuffleCardsProgressStorage;
	protected readonly int _delay;
	private readonly char _incrementProgressKey = '+';
	private readonly char _decrementProgressKey = '-';
	private readonly char _showTranslationKey = '0';

	public string Name => "Inknown Game";

	private record InstructionResult(bool GoToNextWord);

	public ProgressCardsPlayerBase(IVocabularyStorage vocabularyStorage, IMainLog log, IShuffleCardsProgressStorage shuffleCardsProgressStorage, int delay)
	{
		_vocabularyStorage = vocabularyStorage;
		_log = log;
		_shuffleCardsProgressStorage = shuffleCardsProgressStorage;
		_delay = delay;
	}
	public abstract Task<IEnumerable<CardProgress>> GetWords();

	public async Task Play()
	{
		var instructions = BuildInstructionsMap();

		foreach (var word in await GetWords())
		{
			Console.Clear();
			_log.AppendLine($"{word.Card.Literal}");

			while (true)
			{
				var key = Console.ReadKey();
				ClearReadKey();

				if (instructions.TryGetValue(key.KeyChar, out var action))
				{
					if ((await action(word)).GoToNextWord == true)
						break;
				}
				else
					break;
			}
		}
	}

	private Dictionary<char, Func<CardProgress, Task<InstructionResult>>> BuildInstructionsMap() =>
		new()
		{
			{
				_showTranslationKey, async (CardProgress word) =>
				{
					await ShowTranslation(word.Card);
					return new(false);
				}
			},
			{
				_incrementProgressKey, async (CardProgress word) =>
				{
					Increment(word.Card.Literal);
					await ShowTranslation(word.Card);
					await DelayBeforNextCard();
					return new(true);
				}
			},
			{
			_decrementProgressKey, async (CardProgress word) =>
				{
					Decrement(word.Card.Literal);
					await ShowTranslation(word.Card);
					await DelayBeforNextCard();
					return new(true);
				}
			}
		};

	private Task DelayBeforNextCard() => Task.Delay(_delay);


	public string? GetShortcuts() => $"'0' - show translation. '+' - i know the answer. '-' - i don`t know the answer";

	protected static void ClearReadKey()
	{
		if (Console.CursorLeft <= 0)
			return;

		Console.CursorLeft -= 1;
		Console.Write(" ");
		Console.CursorLeft -= 1;
	}

	protected void Decrement(string literal) => _shuffleCardsProgressStorage.Decrement(literal);

	protected void Increment(string literal) => _shuffleCardsProgressStorage.Increment(literal);

	protected async Task ShowTranslation(Word card) => _log.AppendLine($"{new string('\n', 5)}[{_shuffleCardsProgressStorage.Get(card.Literal)}] {card.Translation}");
}