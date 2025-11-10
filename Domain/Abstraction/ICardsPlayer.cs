namespace Domain.Abstraction;

public interface ICardsPlayer
{
	string Name { get; }

    Task PlayAsync();

	string? GetShortcuts();
}
