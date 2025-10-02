using System;
using System.Collections.Generic;
using System.Linq;

namespace Game_Rick_and_Morty
{
    public class ClassicMorty : IMorty
    {
        public string Name => "Classic Morty";
        private Random _internalRandom = new Random();

        public int HidePortalGun(int numberOfBoxes, Sha3BasedRandomGenerator randomnessProtocol, int rickInputForMortyChoice)
        {
            var protocolResult = randomnessProtocol.GenerateFairNumber(numberOfBoxes, rickInputForMortyChoice);
            return protocolResult.FinalValue;
        }

        public List<int> RevealEmptyBoxes(int numberOfBoxes, int rickChosenBox, int portalGunBox, Sha3BasedRandomGenerator randomnessProtocol, int rickInputForMortyChoice)
        {
            List<int> boxesToReveal = new List<int>();
            List<int> eligibleBoxes = new List<int>();

            for (int i = 0; i < numberOfBoxes; i++)
            {
                if (i != rickChosenBox && i != portalGunBox)
                {
                    eligibleBoxes.Add(i);
                }
            }

            int numBoxesToOpen = numberOfBoxes - 2;

            while (boxesToReveal.Count < numBoxesToOpen)
            {
                var protocolResult = randomnessProtocol.GenerateFairNumber(eligibleBoxes.Count, rickInputForMortyChoice);
                int indexToRemove = protocolResult.FinalValue;

                if (indexToRemove >= eligibleBoxes.Count)
                {
                    indexToRemove %= eligibleBoxes.Count;
                }

                int boxIdToReveal = eligibleBoxes[indexToRemove];
                boxesToReveal.Add(boxIdToReveal);
                eligibleBoxes.RemoveAt(indexToRemove);
            }

            return boxesToReveal;
        }

        public bool ShouldMortyEncourageSwitch(int rickChosenBox, int portalGunBox)
        {
            return _internalRandom.Next(2) == 1;
        }

        public double GetTheoreticalWinProbability(int numberOfBoxes, bool rickAlwaysSwitches)
        {
            if (rickAlwaysSwitches)
            {
                return (double)(numberOfBoxes - 1) / numberOfBoxes;
            }
            else
            {
                return 1.0 / numberOfBoxes;
            }
        }
    }
}