using Autofac;
using Common.Log;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Services;
using Newtonsoft.Json;

namespace Domain;

public static class Entry
{
    public static ContainerBuilder AddDomain(this ContainerBuilder builder)
    {
        var fileLog = new FilePrinter("EnglishWords_" + LogUtils.GenerateLogFileName());
        var consoleLog = new ConsolePrinter();
        var mainLog = new MainLog(fileLog, consoleLog);
        builder.RegisterInstance<IMainLog>(mainLog);

        EnsureOptionsFileCreated();

        var options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(Constants.Paths.OptionsPath));

        builder.RegisterInstance<Options>(options);

        builder.RegisterType<VocabularyStorage>().As<IVocabularyStorage>();
        builder.RegisterType<StorageEnricher>().As<IStorageEnricher>();
        builder.RegisterType<QuizCardsPlayer>().As<IQuizPlayer>();
        builder.RegisterType<LeastKnownCardPlayer>().As<ILeastKnownCardPlayer>();
        builder.RegisterType<ShuffleCardsProgressStorage>().As<IShuffleCardsProgressStorage>();

        return builder;
    }

    private static void EnsureOptionsFileCreated()
    {
        if (!File.Exists(Constants.Paths.OptionsPath))
            File.WriteAllText(Constants.Paths.OptionsPath, JsonConvert.SerializeObject(new Options(), Formatting.Indented));
    }

    private static void SaveOptions(Options options) => File.WriteAllText(Constants.Paths.OptionsPath, JsonConvert.SerializeObject(options, Formatting.Indented));
}
