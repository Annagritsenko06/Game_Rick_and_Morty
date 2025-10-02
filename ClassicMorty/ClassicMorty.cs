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

            if (rickChosenBox == portalGunBox)
            {
                List<int> candidatesToKeep = new List<int>();
                for (int i = 0; i < numberOfBoxes; i++)
                {
                    if (i != rickChosenBox)
                    {
                        candidatesToKeep.Add(i);
                    }
                }

                var protocolResult = randomnessProtocol.GenerateFairNumber(candidatesToKeep.Count, rickInputForMortyChoice);
                int keepIndex = protocolResult.FinalValue % candidatesToKeep.Count;
                int keepBox = candidatesToKeep[keepIndex];

                for (int i = 0; i < numberOfBoxes; i++)
                {
                    if (i != rickChosenBox && i != keepBox)
                    {
                        boxesToReveal.Add(i);
                    }
                }
            }
            else
            {
                _ = randomnessProtocol.GenerateFairNumber(numberOfBoxes, rickInputForMortyChoice);

                for (int i = 0; i < numberOfBoxes; i++)
                {
                    if (i != rickChosenBox && i != portalGunBox)
                    {
                        boxesToReveal.Add(i);
                    }
                }
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


