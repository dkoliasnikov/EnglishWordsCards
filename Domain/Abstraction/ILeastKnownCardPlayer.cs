namespace Domain.Abstraction;

public interface ILeastKnownCardPlayer : ICardsPlayer
{
    Task Play();
}
