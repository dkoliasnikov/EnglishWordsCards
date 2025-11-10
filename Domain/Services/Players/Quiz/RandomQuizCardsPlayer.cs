using Common.Log.Abstractions;
using Common.Extensions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services.Players.Quiz;

internal class RandomQuizCardsPlayer : QuizPlayerBase, IRandomQuizPlayer
{
	public override string Name { get => "Quiz"; }

    public RandomQuizCardsPlayer(IVocabularyStorage vocabularyStorage, IMainLog log, Options options) : base(vocabularyStorage, log, options)
    {
        
    }

    protected override async Task<ICollection<Word>> GetWordsAsync()
    {
        var vocabulary = await VocabularyStorage.GetVocabularyAsync();

        return vocabulary.Words.Shuffle().ToList();
    }
}