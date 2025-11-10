namespace Domain;

public record Options(
    TimeSpan DelayBeforeNextCard,
    string VocabularyStoragePath = Constants.Paths.VocabularyStoragePath,
    string WordsSeedPath = Constants.Paths.WordsSeedPath,
    string ShuffleCardsProgressStoragePath = Constants.Paths.ShuffleCardsProgressStoragePath
);