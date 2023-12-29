using Domain.Models;

namespace Domain.Abstraction;

public interface IVocabularyStorage
{
    Task<Vocabulary> GetVocabularyAsync();

    Task AddNewWordsAsync(IEnumerable<Word> words);
}
