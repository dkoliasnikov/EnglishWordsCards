using Common.Log.Abstractions;
using Common.Extensions;
using Domain.Abstraction;
using Domain.Models;
using Domain.Exceptions;

namespace Domain.Services.Players.Quiz;

internal class LeastKnownQuizPlayer : QuizPlayerBase, ILeastKnownQuizPlayer
{
	public override string Name { get => "Least Known Quiz (not affects progress)"; }

	private readonly IShuffleCardsProgressStorage _shuffleCardsProgressStorage;

    public LeastKnownQuizPlayer(IShuffleCardsProgressStorage shuffleCardsProgressStorage, IMainLog log, Options options, IVocabularyStorage vocabularyStorage) : base(vocabularyStorage, log, options)
    {
        _shuffleCardsProgressStorage = shuffleCardsProgressStorage;
    }

    protected override async Task<ICollection<Word>> GetWordsAsync()
    {
        var vocabulary = await VocabularyStorage.GetVocabularyAsync();

        return vocabulary
            .Words
            .Select(w => new CardProgress(w, _shuffleCardsProgressStorage.Get(w.Literal) ?? throw new MissingWordProgressException(w.Literal)))
            .OrderBy(w => w.ProgressValue)
            .Select(w => w.Card)
            .ToList();
    }
}