using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players;

internal class LeastKnownCardPlayer : ProgressCardsPlayerBase, ILeastKnownCardPlayer
{
    public override string Name { get => "Least known card"; }

    public LeastKnownCardPlayer(IVocabularyStorage vocabularyStorage, IMainLog log, IShuffleCardsProgressStorage shuffleCardsProgressStorage, TimeSpan delayBeforeNextCard)
        : base(vocabularyStorage, log, shuffleCardsProgressStorage, delayBeforeNextCard)
    {
    }

    public override async Task<List<CardProgress>> GetWords() => (await _vocabularyStorage.GetVocabularyAsync())
            .Words
            .Select(w => new CardProgress(w, _shuffleCardsProgressStorage.Get(w.Literal)))
            .OrderBy(w => w.ProgressValue)
            .ToList();
}