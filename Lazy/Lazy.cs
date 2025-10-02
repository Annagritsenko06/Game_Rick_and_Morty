using System;
using System.Collections.Generic;
using System.Linq;
using Game_Rick_and_Morty;

namespace RickAndMortyGame.MortyImplementations
{
    public class Lazy : IMorty
    {
        public string Name => "Lazy";
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
            eligibleBoxes.Sort();
            for (int i = 0; i < numBoxesToOpen && i < eligibleBoxes.Count; i++)
            {
                boxesToReveal.Add(eligibleBoxes[i]);
            }
            return boxesToReveal;
        }

        public bool ShouldMortyEncourageSwitch(int rickChosenBox, int portalGunBox)
        {
            return rickChosenBox == portalGunBox;
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


