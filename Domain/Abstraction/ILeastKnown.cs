namespace Domain.Abstraction;

public interface ILeastKnown : ICardsPlayer
{
    Task Play();
}
