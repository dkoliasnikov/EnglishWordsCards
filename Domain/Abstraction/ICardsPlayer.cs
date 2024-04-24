namespace Domain.Abstraction;

public interface ICardsPlayer
{
	string Name { get; }

    Task Play();

	string? GetShortcuts();
}
