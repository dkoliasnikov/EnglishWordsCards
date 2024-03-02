namespace Domain.Abstraction;

public interface ICardsPlayer
{
    Task Play();

	string? GetShortcuts();
}
