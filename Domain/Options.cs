namespace Domain;

public record Options(
    string VocabularyStoragePath = Constants.Paths.VocabularyStoragePath,
    string WordsSeedPath = Constants.Paths.WordsSeedPath,
    string ShuffleCardsProgressStoragePath = Constants.Paths.ShuffleCardsProgressStoragePath,
    int DelayBeforeNextCard = 2000
);