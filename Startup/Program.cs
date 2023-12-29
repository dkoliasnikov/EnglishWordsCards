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
        var enricher = scope.Resolve<IStorageEnricher>();
        var shuffleCardsPlayer = scope.Resolve<IShuffleCardsPlayer>();
        await shuffleCardsPlayer.Play();
    }
    catch (Exception e)
    {
        await log.Error("Error", e);
    }
    finally
    {
        Console.WriteLine("Done");
    }
}

