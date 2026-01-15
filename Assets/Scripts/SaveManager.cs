using UnityEngine;
using System.IO;
using System.Collections.Generic;

//SaveManager.Instance.SaveGame();
//SaveManager.Instance.LoadGame();


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    string path;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        path = Application.persistentDataPath + "/save.json";
    }

    // ================= SAVE =================

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // ---- basic ----
        data.currentDay = DayManager.Instance.currentDay;
        data.coins = CoinManager.Instance.coins;
        data.unlockedPlants = DayManager.Instance.unlockedPlants;

        data.rainChanceBasePercent = DayManager.Instance.rainChancePercent;
        data.stormChanceBasePercent = DayManager.Instance.stormChanceBasePercent;

        // ---- streaks ----
        data.accountingStreak = PlayerPrefs.GetInt("AccountingStreak", 0);
        data.cleaningStreak = PlayerPrefs.GetInt("CleaningStreak", 0);

        // ---- plots ----
        data.plots = new List<PlotSaveData>();

        if (DayManager.Instance.allPlots != null)
        {
            foreach (var plot in DayManager.Instance.allPlots)
            {
                if (plot == null) continue;

                string key = $"{plot.transform.position.x}_{plot.transform.position.y}";
                string plantType = plot.GetPlantedPlant() != null ? plot.GetPlantedPlant().seedType : "";

                PlotSaveData p = new PlotSaveData
                {
                    key = key,
                    plantType = plantType,
                    growthStage = plot.growthStage,
                    isWatered = plot.isWatered,
                    dead = plot.dead,
                    composted = plot.composted,
                    isShiny = plot.isShiny,
                    isGrape = plot.isGrape,
                    grapeMaxHarvests = plot.grapeMaxHarvests,
                    grapeHarvestsDone = plot.grapeHarvestsDone
                };

                data.plots.Add(p);
            }
        }

        // ---- composter ----
        var comp = FindFirstObjectByType<Composter>();
        if (comp != null)
        {
            data.composterIsFull = comp.isFull;
            data.composterIsReady = comp.isReady;
            data.composterIsTrashcan = comp.isTrashcan;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved game");
    }

    // ================= LOAD =================

    public void LoadGame()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // ---- basic ----
        DayManager.Instance.currentDay = data.currentDay;
        CoinManager.Instance.coins = data.coins;
        DayManager.Instance.unlockedPlants = data.unlockedPlants;

        DayManager.Instance.rainChancePercent = data.rainChanceBasePercent;
        DayManager.Instance.stormChanceBasePercent = data.stormChanceBasePercent;

        // ---- streaks ----
        PlayerPrefs.SetInt("AccountingStreak", data.accountingStreak);
        PlayerPrefs.SetInt("CleaningStreak", data.cleaningStreak);

        // ---- plots ----
        if (data.plots != null)
        {
            DayManager.Instance.LoadPlotsFromSave(data.plots);
        }

        // ---- composter ----
        DayManager.Instance.LoadComposterFromSave(
            data.composterIsFull,
            data.composterIsReady,
            data.composterIsTrashcan
        );

        Debug.Log("Loaded game");
    }
}