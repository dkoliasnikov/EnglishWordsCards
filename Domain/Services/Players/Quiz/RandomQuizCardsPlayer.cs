using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players.Quiz;

internal class RandomQuizCardsPlayer : QuizPlayerBase, IRandomQuizPlayer
{
    public override string Name { get => "Quiz"; }

    public RandomQuizCardsPlayer(IVocabularyStorage vocabularyStorage, IShuffleCardsProgressStorage shuffleCardsProgressStorage, IMainLog log, Options options) 
        : base(vocabularyStorage, shuffleCardsProgressStorage, log, options)
    {
    }

    protected override async Task<ICollection<Word>> GetWordsAsync()
    {
        var vocabulary = await VocabularyStorage.GetVocabularyAsync();

        return vocabulary.Words.Shuffle().ToList();
    }
}