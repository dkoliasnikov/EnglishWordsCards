
namespace Domain.Abstraction;

public interface IShuffleCardsProgressStorage
{
    void Put((string literal, int value) cardProgress);

    int Get(string literal);

    void Increment(string literal);
   
    void Decrement(string literal);
}
