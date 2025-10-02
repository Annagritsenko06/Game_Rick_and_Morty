using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;

namespace Game_Rick_and_Morty
{
    public class GameSettings
    {
        public int NumberOfBoxes { get; set; } = 3;
        public int NumberOfRounds { get; set; } = 1;
        public string MortyType { get; set; } = "ClassicMorty";
    }
    public class ArgsHandler
    {
        public static GameSettings Parse(string[] args)
        {
            var settings = new GameSettings();

            if (args != null && args.Length > 0)
            {
                if (int.TryParse(args[0], out int boxesArg))
                {
                    if (boxesArg >= 3)
                    {
                        settings.NumberOfBoxes = boxesArg;
                    }
                    else
                    {
                        Console.WriteLine("Warning: Number of boxes must be at least 3. Using default (3).");
                    }
                }
                else
                {
                    Console.WriteLine("Error: First argument must be an integer for number of boxes (>=3).");
                    Environment.Exit(1);
                }

                if (args.Length > 1)
                {
                    string mortyArg = args[1];
                    if (string.Equals(mortyArg, "classic", StringComparison.OrdinalIgnoreCase)) mortyArg = "ClassicMorty";
                    if (string.Equals(mortyArg, "lazy", StringComparison.OrdinalIgnoreCase)) mortyArg = "LazyMorty";

                    if (string.Equals(mortyArg, "ClassicMorty", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(mortyArg, "LazyMorty", StringComparison.OrdinalIgnoreCase))
                    {
                        settings.MortyType = mortyArg;
                    }
                    else
                    {
                        Console.WriteLine("Error: Morty type must be 'ClassicMorty' or 'LazyMorty' (or aliases 'classic'/'lazy').");
                        Environment.Exit(1);
                    }
                }
            }
            return settings;
        }

        
        public void DisplayGameResult(bool rickWon, int roundNumber)
        {
            Console.WriteLine($"--- Round {roundNumber} ---");
            Console.WriteLine(rickWon ? "Rick won the portal gun! Congratulations!" : "Morty won. The portal gun was not found.");
        }




      
    public void DisplayProof(byte[] secretKey, int mortyValue, int finalValue)
    {
        var table = new ConsoleTable("Indicator", "Value");

        table.AddRow("Morty secret key (Base64)", Convert.ToBase64String(secretKey))
             .AddRow("Morty random value", mortyValue)
             .AddRow("Final calculated random value", finalValue);

        Console.WriteLine("\n--- Proof of Randomness ---");
        table.Write();
    }
    }
}