using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int currentDay;
    public int coins;

    public int accountingStreak;
    public int packingStreak;
    public int cleaningStreak;
    public int catStreak;

    public bool shinyTalismanAvailable;
    public int unlockedPlants;

    public float rainChanceBasePercent;
    public float stormChanceBasePercent;

    // plot states
    public List<PlotSaveData> plots;

    // composter
    public bool composterIsFull;
    public bool composterIsReady;
    public bool composterIsTrashcan;
}
