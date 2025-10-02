using System;
using Game_Rick_and_Morty;
using RickAndMortyGame.GameCore;

namespace RickAndMortyGame
{
    class Program
    {
        static void Main(string[] args)
        {
            GameSettings settings = ArgsHandler.Parse(args);
            GameEngine game = new GameEngine(settings);
            game.StartGame();
        }
    }
}