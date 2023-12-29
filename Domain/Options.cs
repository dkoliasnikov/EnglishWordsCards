namespace Domain;

public record Options (
    string VocabularyStoragePath = Constants.Paths.VocabularyStoragePath, 
    string WordsSeedPath = Constants.Paths.WordsSeedPath
);