using Autofac;
using Common.Log;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Constants;
using Domain.Services;
using Domain.Services.Players;
using Domain.Services.Players.Quiz;
using Domain.Services.Storage;
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
        builder.RegisterType<ShuffleCardsProgressStorage>().As<IShuffleCardsProgressStorage>();

        builder.RegisterType<RandomQuizCardsPlayer>().As<IRandomQuizPlayer>()
            .WithParameter(ParameterNames.DelayBeforeNextCard, options.DelayBeforeNextCard);

        builder.RegisterType<LeastKnownQuizPlayer>().As<ILeastKnownQuizPlayer>()
            .WithParameter(ParameterNames.DelayBeforeNextCard, options.DelayBeforeNextCard);

        builder.RegisterType<LeastKnownCardPlayer>().As<ILeastKnownCardPlayer>()
            .WithParameter(ParameterNames.DelayBeforeNextCard, options.DelayBeforeNextCard);

        builder.RegisterType<RandomCardPlayer>().As<IRandomCardPlayer>()
            .WithParameter(ParameterNames.DelayBeforeNextCard, options.DelayBeforeNextCard);

        return builder;
    }

    private static void EnsureOptionsFileCreated()
    {
        if (!File.Exists(Constants.Paths.OptionsPath))
            File.WriteAllText(path: Constants.Paths.OptionsPath, JsonConvert.SerializeObject(new Options(Constants.Options.DefaultDelayBeforeNextCard), Formatting.Indented));
    }
}
