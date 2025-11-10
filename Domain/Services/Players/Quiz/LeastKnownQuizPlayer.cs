using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players.Quiz;

internal class LeastKnownQuizPlayer : QuizPlayerBase, ILeastKnownQuizPlayer
{
    public override string Name { get => "Least Known Quiz"; }

    public LeastKnownQuizPlayer(IMainLog log, IVocabularyStorage vocabularyStorage, IShuffleCardsProgressStorage shuffleCardsProgressStorage, TimeSpan delayBeforeNextCard) 
        : base(vocabularyStorage, shuffleCardsProgressStorage, log, delayBeforeNextCard)
    {
    }

    protected override async Task<List<Word>> GetWordsAsync()
    {
        var vocabulary = await VocabularyStorage.GetVocabularyAsync();

        return vocabulary
            .Words
            .Select(w => new CardProgress(w, ShuffleCardsProgressStorage.Get(w.Literal)))
            .OrderBy(w => w.ProgressValue)
            .Select(w => w.Card)
            .ToList();
    }
}