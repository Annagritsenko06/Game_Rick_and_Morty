using System.Collections.Generic;

namespace Game_Rick_and_Morty
{
    public interface IMorty
    {
        string Name { get; }
        int HidePortalGun(int numberOfBoxes, Sha3BasedRandomGenerator randomnessProtocol, int rickInputForMortyChoice);
        List<int> RevealEmptyBoxes(int numberOfBoxes, int rickChosenBox, int portalGunBox, Sha3BasedRandomGenerator randomnessProtocol, int rickInputForMortyChoice);
        bool ShouldMortyEncourageSwitch(int rickChosenBox, int portalGunBox);
        double GetTheoreticalWinProbability(int numberOfBoxes, bool rickAlwaysSwitches);
    }
}