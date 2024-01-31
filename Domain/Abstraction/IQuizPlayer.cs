namespace Domain.Abstraction;

public interface IQuizPlayer : ICardsPlayer
{
    Task Play();
}
