﻿using Common.Extensions;
using Common.Log.Abstractions;
using Domain.Abstraction;
using Domain.Models;

namespace Domain.Services;

internal class RandomCardPlayer : ProgressCardsPlayerBase, IRandomCardPlayer
{
	public string Name { get => "Random Card"; }

	public RandomCardPlayer(IVocabularyStorage vocabularyStorage, IMainLog log, IShuffleCardsProgressStorage shuffleCardsProgressStorage, int delay)
		: base(vocabularyStorage, log, shuffleCardsProgressStorage, delay)
	{
	}

	public override async Task<IEnumerable<CardProgress>> GetWords() =>
			(await _vocabularyStorage.GetVocabularyAsync())
			.Words
			.Select(w => new CardProgress(w, _shuffleCardsProgressStorage.Get(w.Literal)))
			.Shuffle();
}