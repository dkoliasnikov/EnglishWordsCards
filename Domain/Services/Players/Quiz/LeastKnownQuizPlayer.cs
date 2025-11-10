using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players.Quiz;

internal class LeastKnownQuizPlayer : QuizPlayerBase, ILeastKnownQuizPlayer
{
    public override string Name { get => "Least Known Quiz"; }

    public LeastKnownQuizPlayer(IMainLog log, Options options, IVocabularyStorage vocabularyStorage, IShuffleCardsProgressStorage shuffleCardsProgressStorage) 
        : base(vocabularyStorage, shuffleCardsProgressStorage, log, options)
    {
    }

    protected override async Task<ICollection<Word>> GetWordsAsync()
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