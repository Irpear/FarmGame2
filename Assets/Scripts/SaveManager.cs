using UnityEngine;
using System.IO;

//SaveManager.SaveGame();
//SaveManager.LoadGame();

// Wat wordt waar opgeslagen:
// - save.json:   alles wat in een variabele van DayManager / CoinManager / SeedManager leeft
//                (dag, coins, seeds, grondstoffen, plots, composter, kansen op regen/storm/shiny)
// - PlayerPrefs: unlocks, aankopen, streaks, kippen, eieren, feeders, processor en shop stock
//                (die scripts lezen en schrijven dat zelf al rechtstreeks)
// SaveGame() schrijft beide tegelijk weg, zodat ze altijd bij elkaar passen.
//
// Er wordt opgeslagen: aan het eind van de nacht, bij elke scenewissel en wanneer de app
// naar de achtergrond gaat of afsluit (zie DayManager).

public static class SaveManager
{
    private static string SavePath => Application.persistentDataPath + "/save.json";

    // ================= SAVE =================

    public static void SaveGame()
    {
        if (DayManager.Instance == null || CoinManager.Instance == null || SeedManager.Instance == null)
            return;

        // Tijdens de nacht is de nieuwe dag nog niet af, DayManager slaat zelf op als alles verwerkt is
        if (DayManager.Instance.isEndingDay)
            return;

        SaveData data = new SaveData();

        DayManager.Instance.SaveTo(data);
        CoinManager.Instance.SaveTo(data);
        SeedManager.Instance.SaveTo(data);

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        PlayerPrefs.Save();

        Debug.Log("Saved game");
    }

    // ================= LOAD =================

    public static void LoadGame()
    {
        if (!File.Exists(SavePath)) return;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        if (data == null) return;

        DayManager.Instance.LoadFrom(data);
        CoinManager.Instance.LoadFrom(data);
        SeedManager.Instance.LoadFrom(data);

        Debug.Log("Loaded game");
    }

    // ================= DELETE =================

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
