using Autofac;
using Common.Log.Abstractions;
using Domain;
using Domain.Abstraction;

var builder = new ContainerBuilder();

builder.AddDomain();

var ioc = builder.Build();

using (var scope = ioc)
{
    var log = scope.Resolve<IMainLog>();

    try
    {
        ICardsPlayer player = LetUserChooseGameMode(scope, log);

        await player.Play();
    }
    catch (Exception e)
    {
        await log.Error("Error", e);
        Console.ReadKey();
    }
    finally
    {
        Console.WriteLine("Done");
    }
}

static ICardsPlayer LetUserChooseGameMode(IContainer scope, IMainLog log)
{

    log.AppendLine($"Choose game mode\n\n1 - Quiz\n\n2 - Cards shuffle");
    return Console.ReadKey().KeyChar == '1' ? scope.Resolve<IQuizPlayer>() : scope.Resolve<IShuffleCardsPlayer>();
}