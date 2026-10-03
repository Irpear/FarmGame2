using UnityEngine;

// Let op: de nummers worden in de Inspector opgeslagen, dus niet van volgorde veranderen.
// Nieuwe trofee? Onderaan toevoegen met een nieuw nummer.
public enum TrophyId
{
    EarnInOneDay = 0,
    FullField = 1,
    TotalHarvests = 2,
    BuyEverything = 3,
    MinigameStreak = 4
}

// Houdt bij welke trofeeën behaald zijn (PlayerPrefs "trophy_<naam>").
// Doelen (300 coins, 500 oogsten) staan op DayManager zodat je ze in de Inspector kunt aanpassen.
//
// Trophies.CheckProfit();
// Trophies.AddHarvest();
// Trophies.CheckFullField(DayManager.Instance.allPlots);
// Trophies.CheckBuyEverything();
// Trophies.RecordMinigameResult(Correct);

public static class Trophies
{
    private const string HarvestCountKey = "total_harvests";
    private const string MinigameStreakKey = "minigame_win_streak";

    private static string Key(TrophyId id) => $"trophy_{id}";

    public static bool IsUnlocked(TrophyId id)
    {
        return PlayerPrefs.GetInt(Key(id), 0) == 1;
    }

    private static void Unlock(TrophyId id)
    {
        if (IsUnlocked(id)) return;

        PlayerPrefs.SetInt(Key(id), 1);
        PlayerPrefs.Save();
        Debug.Log($"Trophy unlocked: {id}");

        NotificationManager.Instance?.ShowNotification("You earned a trophy! Check the barn.", 3f);

        if (AllUnlocked())
            CompletionPopup.OnGameCompleted();
    }

    public static bool AllUnlocked()
    {
        foreach (TrophyId id in System.Enum.GetValues(typeof(TrophyId)))
        {
            if (!IsUnlocked(id)) return false;
        }
        return true;
    }

    public static string GetDescription(TrophyId id)
    {
        return id switch
        {
            TrophyId.EarnInOneDay => $"Earn {DayManager.Instance.trophyProfitGoal} coins in one day",
            TrophyId.FullField => "Fill every plot with the same crop",
            TrophyId.TotalHarvests => $"Harvest {DayManager.Instance.trophyHarvestGoal} crops",
            TrophyId.BuyEverything => "Buy everything there is to buy",
            TrophyId.MinigameStreak => $"Win {DayManager.Instance.trophyMinigameStreakGoal} minigames in a row",
            _ => ""
        };
    }

    // ---- 1. Coins in één dag ----
    public static void CheckProfit()
    {
        if (CoinManager.Instance.profit >= DayManager.Instance.trophyProfitGoal)
            Unlock(TrophyId.EarnInOneDay);
    }

    // ---- 2. Alle plots dezelfde (levende) plant ----
    public static void CheckFullField(Plot[] plots)
    {
        if (plots == null || plots.Length == 0) return;

        string type = null;

        foreach (var plot in plots)
        {
            if (plot == null) continue;

            PlantData plant = plot.GetPlantedPlant();
            if (plant == null || plot.dead) return;

            if (type == null) type = plant.seedType;
            else if (plant.seedType != type) return;
        }

        Unlock(TrophyId.FullField);
    }

    // ---- 3. Totaal aantal oogsten ----
    public static void AddHarvest()
    {
        int count = PlayerPrefs.GetInt(HarvestCountKey, 0) + 1;
        PlayerPrefs.SetInt(HarvestCountKey, count);

        if (count >= DayManager.Instance.trophyHarvestGoal)
            Unlock(TrophyId.TotalHarvests);
    }

    // ---- 5. Minigames achter elkaar gewonnen (alle soorten samen) ----
    // Aanroepen aan het eind van elke minigame, verliezen zet de teller terug op 0
    public static void RecordMinigameResult(bool won)
    {
        int streak = won ? PlayerPrefs.GetInt(MinigameStreakKey, 0) + 1 : 0;
        PlayerPrefs.SetInt(MinigameStreakKey, streak);
        PlayerPrefs.Save();

        if (streak >= DayManager.Instance.trophyMinigameStreakGoal)
            Unlock(TrophyId.MinigameStreak);
    }

    // ---- 4. Alles gekocht ----
    // Aanroepen na elke aankoop
    public static void CheckBuyEverything()
    {
        string[] required =
        {
            // farm + schuur
            "barn_unlocked",
            "coop_unlocked",
            "all_plots_unlocked",
            // vuilnis in de schuur (moet gelijk zijn aan de Pile ID's in de Inspector)
            "trashPile_1",
            "trashPile_2",
            // upgrades
            "composter_unlocked",
            "scythe_unlocked",
            "processor_unlocked",
            "feeder2_unlocked",
            "plantbook_unlocked",
            // talismannen
            "rainTalisman_maxed",
            "StormTalismanPurchased",
            "ShinyTalismanPurchased",
            // dieren
            "chicken1_unlocked",
            "chicken2_unlocked"
        };

        foreach (string key in required)
        {
            if (PlayerPrefs.GetInt(key, 0) != 1) return;
        }

        Unlock(TrophyId.BuyEverything);
    }
}
