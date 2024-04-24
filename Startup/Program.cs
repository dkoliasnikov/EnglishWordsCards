﻿using Autofac;
using Common.Log.Abstractions;
using Domain;
using Domain.Abstraction;
using Startup.Utils;
using System.Reflection;


var builder = new ContainerBuilder();

builder.AddDomain();

var ioc = builder.Build();

using (var scope = ioc)
{
	var log = scope.Resolve<IMainLog>();

	try
	{
		ConsoleConfigurator.Configure();

		ICardsPlayer player = LetUserChooseGameMode(scope, log);

		Console.Clear();

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

static string FormatPlayerOption(PlayerOption playerOption) => $"{playerOption.ShortCut} - {playerOption.Player.Name} (shortcuts: {playerOption.Player.GetShortcuts()})";

static ICardsPlayer LetUserChooseGameMode(IContainer scope, IMainLog log)
{
	var gameOptionInstance = GetGamesOptionsMap(scope);

	log.AppendLine($"Choose game mode:\n\n{string.Join("\r\n", gameOptionInstance.Select(FormatPlayerOption))}");

	var userKey = Console.ReadKey().KeyChar;

	if (gameOptionInstance.ToDictionary(x => x.ShortCut).TryGetValue(userKey, out var gameOption))
	{
		return gameOption.Player;
	}

	throw new($"Failed to select game for option: {userKey}");
}

static IEnumerable<PlayerOption> GetGamesOptionsMap(IContainer scope)
{
	var allCardsGamesTypes = GetCardsGameImplementation();

	var shortCutsPool = Enumerable.Range(0, 10).Select(i => i.ToString().Single()).ToArray();

	var gameOptionInstance = allCardsGamesTypes.Select((type, index) =>
	{
		var player = scope.Resolve(type.PlayerInterfaceType) as ICardsPlayer;

		if (player is null)
			throw new ArgumentNullException($"Failed to cast from \"{type.GetType().FullName}\" to {typeof(ICardsPlayer).FullName}");

		return new PlayerOption(player, shortCutsPool[index]);
	});

	return gameOptionInstance;
}

static string? GetCardsGameName(ICardsPlayer t) =>
		(string?)t.GetType()
		.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
		.Where(info => info.Name == "Name")
		.Single()
		.GetValue(null);

static IEnumerable<ICardsPlayer> GetAllCardsPlayer() => GetCardsGameImplementation().Cast<ICardsPlayer>();

static IEnumerable<PlayerWithInterface> GetCardsGameImplementation() =>
		 AppDomain.CurrentDomain.GetAssemblies()
		.SelectMany(s => s.GetTypes())
		.Where(type => typeof(ICardsPlayer).IsAssignableFrom(type) && !type.IsAbstract).Select(t =>
			new PlayerWithInterface(t, t.GetInterfaces().Where(i => i.Name != nameof(ICardsPlayer)).Single()));

internal record struct PlayerOption(ICardsPlayer Player, char ShortCut);

internal record struct PlayerWithInterface(Type PlayerType, Type PlayerInterfaceType);