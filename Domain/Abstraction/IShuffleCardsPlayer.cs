﻿namespace Domain.Abstraction;

public interface IShuffleCardsPlayer : ICardsPlayer
{
    Task Play();
}
