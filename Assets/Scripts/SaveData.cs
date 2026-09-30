using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // ---- DayManager ----
    public int currentDay;
    public int unlockedPlants;
    public bool taskLeft;

    public float rainChanceBasePercent;
    public float stormChanceBasePercent;
    public float shinyChanceBasePercent;

    // plot states
    public List<PlotSaveData> plots;

    // composter
    public bool composterIsFull;
    public bool composterIsReady;
    public bool composterIsTrashcan;

    // ---- CoinManager ----
    public int coins;
    public int profit;
    public int wheatResource;
    public int cornResource;
    public int animalFood;
    public int animalFood2;

    // ---- SeedManager ----
    public List<SeedSaveData> seeds;
}

[System.Serializable]
public class SeedSaveData
{
    public string seedType;
    public int amount;
}
