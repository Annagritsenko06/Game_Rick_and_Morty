using ConsoleTables;
using System;

namespace Game_Rick_and_Morty
{
    internal class GameStatistics
    {
        public int TotalRoundsPlayed { get; private set; }
        public int RickWins { get; private set; }
        public int RickLosses { get; private set; }


        public void RecordGameResult(bool rickWon)
        {
            TotalRoundsPlayed++;
            if (rickWon)
            {
                RickWins++;
            }
            else
            {
                RickLosses++;
            }
        }

        public void Reset()
        {
            TotalRoundsPlayed = 0;
            RickWins = 0;
            RickLosses = 0;
        }

       

public void DisplayStatistics(int numberOfBoxes, IMorty morty)
    {
        var table = new ConsoleTable("Statistic", "Value");

        table.AddRow("Total rounds played", TotalRoundsPlayed)
             .AddRow("Rick's wins", RickWins)
             .AddRow("Morty's wins", RickLosses)
             .AddRow("Rick's win percentage", TotalRoundsPlayed > 0 ? $"{(double)RickWins / TotalRoundsPlayed * 100:F2}%" : "0%");

        Console.WriteLine("\n--- GAME STATISTICS ---");
        table.Write();

        var probTable = new ConsoleTable("Scenario", "Rick's winning probability");

        probTable.AddRow($"If Rick always switches (Morty: {morty.Name})", $"{morty.GetTheoreticalWinProbability(numberOfBoxes, true) * 100:F2}%")
                 .AddRow($"If Rick always sticks (Morty: {morty.Name})", $"{morty.GetTheoreticalWinProbability(numberOfBoxes, false) * 100:F2}%");

        Console.WriteLine("\n--- THEORETICAL PROBABILITIES ---");
        probTable.Write();

        Console.WriteLine("Note: These probabilities are based on the specific Morty's behavior (if he influences player choice).");
        Console.WriteLine("-----------------------");
    }


}
}
