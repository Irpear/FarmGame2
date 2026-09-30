using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public int currentDay = 1;
    public TextMeshProUGUI dayButtonText;

    public TextMeshProUGUI profitSummary;

    //// Event: scripts kunnen hiernaar luisteren
    //public event Action OnDayEnded;

    public NightTransition nightTransition;

    public Plot[] allPlots;

    // key = "x_y" positie van de plot
    private Dictionary<string, PlotSaveData> plotStates = new Dictionary<string, PlotSaveData>();

    public float rainChancePercent = 0;
    public float stormChanceBasePercent = 2;
    public float stormChancePercent = 2;
    public float shinyChanceBasePercent = 1;

    public bool anyPlantsEaten = false;

    public int unlockedPlants = 1;

    public int currentHighscore = 1;

    public bool taskLeft = true;

    // true zolang de nacht bezig is, dan wordt er niet tussendoor opgeslagen
    public bool isEndingDay = false;

    private bool saveLoaded = false;

    private ComposterState composterState;

    [System.Serializable]
    private struct ComposterState
    {
        public bool isFull;
        public bool isReady;
        public bool isTrashcan;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rainChancePercent = PlayerPrefs.GetFloat("RainChancePercent", 0f);
            bool bought = PlayerPrefs.GetInt("StormTalismanPurchased", 0) == 1;
            if (bought)
            {
                stormChanceBasePercent += -1;  
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (PlayerPrefs.GetInt("first_launch", 1) == 1)
            {
                PlayerPrefs.SetInt("totalUnlockedPlots", 6);
                PlayerPrefs.SetInt("shop_stock_carrot", 6);
                PlayerPrefs.SetInt("first_launch", 0);
                PlayerPrefs.Save();
            }
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    // App gaat naar de achtergrond (op mobiel vaak de laatste kans om op te slaan)
    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveManager.SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveManager.SaveGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
        // Eerste keer laden: pas hier, want dan hebben CoinManager en SeedManager hun Awake ook gehad
        if (!saveLoaded)
        {
            SaveManager.LoadGame();
            saveLoaded = true;
            CoinManager.Instance.UpdateUI();
        }

        ReturnHeldCompost();

        SeedSelectionUI.ActiveSelectedPlant = null;
        SeedSelectionUI.ActiveSelectedTool = null;
        SeedSelectionUI.Instance.ReturnWateringCan();
        SeedSelectionUI.Instance.ReturnScythe();

        var comp = FindFirstObjectByType<Composter>();
        RestoreComposterState(comp);


        // Zoek de UI
        if (dayButtonText == null)
        dayButtonText = GameObject.Find("DayButtonText")?.GetComponent<TextMeshProUGUI>();

        if (profitSummary == null)
            profitSummary = GameObject.Find("ProfitSummary")?.GetComponent<TextMeshProUGUI>();

        // Zoek de button
        var btn = GameObject.Find("DayButton")?.GetComponent<UnityEngine.UI.Button>();


    if (btn != null)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(EndDay);
    }


    if (nightTransition == null)
    {
        nightTransition = GameObject.FindFirstObjectByType<NightTransition>();
        //if (nightTransition == null)
        //    Debug.LogWarning("No NightTransition found in scene!");
    }

        allPlots = FindObjectsByType<Plot>(FindObjectsSortMode.None);
        Debug.Log($"Found {allPlots.Length} plots in scene");

    RestorePlotStates();

    UpdateUI();

    SaveManager.SaveGame();
}

    public void SavePlotStates()
    {
        if (SceneManager.GetActiveScene().name != "FarmScene") // als ergens anders plots komen moet die scene hierbij
        {
            Debug.Log("Not in farm scene - skipping save");
            return;
        }

        plotStates.Clear();

        if (allPlots != null)
        {
            foreach (var plot in allPlots)
            {
                if (plot != null)
                {
                    // Gebruik de positie als unieke identifier
                    string key = $"{plot.transform.position.x}_{plot.transform.position.y}"; 
                    string plantType = plot.GetPlantedPlant() != null ? plot.GetPlantedPlant().seedType : "";

                    plotStates[key] = new PlotSaveData
                    {
                        key = key,
                        plantType = plantType,
                        growthStage = plot.growthStage,
                        isWatered = plot.isWatered,
                        dead = plot.dead,
                        composted = plot.composted,
                        isShiny = plot.isShiny,
                        chosenVariant = plot.chosenVariant,
                        isGrape = plot.isGrape,
                        grapeMaxHarvests = plot.grapeMaxHarvests,
                        grapeHarvestsDone = plot.grapeHarvestsDone
                    };
                }
            }
        }

        Debug.Log($"Saved {plotStates.Count} plot states");
    }

    // Herstel de plot states
    private void RestorePlotStates()
    {
        if (allPlots == null || plotStates.Count == 0)
            return;

        int restored = 0;
        foreach (var plot in allPlots)
        {
            if (plot != null)
            {
                string key = $"{plot.transform.position.x}_{plot.transform.position.y}";
                if (plotStates.ContainsKey(key))
                {
                    var state = plotStates[key];

                    plot.isWatered = state.isWatered;
                    

                    // Als er geen plant in zat → skip
                    if (string.IsNullOrEmpty(state.plantType))
                    {
                        plot.SetEmpty();
                        plot.composted = state.composted;  // ← Voeg toe!
                        plot.UpdateSprite();
                        continue;
                    }

                    // Zoek de juiste PlantData
                    PlantData plant = SeedSelectionUI.Instance.GetPlantDataByType(state.plantType);

                    plot.ForcePlantState(plant, state.growthStage);
                    
                    plot.dead = state.dead;

                    plot.composted = state.composted;

                    plot.isShiny = state.isShiny;

                    plot.chosenVariant = state.chosenVariant;

                    plot.isGrape = state.isGrape;

                    plot.grapeMaxHarvests = state.grapeMaxHarvests;

                    plot.grapeHarvestsDone = state.grapeHarvestsDone;

                    plot.UpdateSprite();

                    restored++;
                }
            }
        }

        Debug.Log($"Restored {restored} plot states");
    }

    public void EndDay()
    {
        if (isEndingDay) return;
        isEndingDay = true;

        Debug.Log("Day " + currentDay + " ended.");

        currentDay++;

        taskLeft = true;

        ProfitHighscoreCheck();

        nightTransition.PlayTransition(() => {

            Debug.Log("EndDay called. Now day is: " + currentDay);

        });

        //events
        bool rain = Random.value <= (rainChancePercent / 100f);

        RecalculateStormChance();

        bool storm = Random.value <= (stormChancePercent / 100f);

        CheckPlantBook();

        ShopManager.ResetDailyStock();

        StartCoroutine(NightRoutine(rain, storm));
    }

    private void ProfitHighscoreCheck()
    {
        currentHighscore = PlayerPrefs.GetInt("myHighscore", 0);
        if (currentHighscore < CoinManager.Instance.profit)
        {
            profitSummary.text = $"You earned {CoinManager.Instance.profit} coins today, which means you got a new highscore!";
            NotificationManager.Instance.ShowNotification("Congratulations on the new highscore", 5f);
            PlayerPrefs.SetInt("myHighscore", CoinManager.Instance.profit);
        }
        else
        {
            profitSummary.text = $"You earned {CoinManager.Instance.profit} coins today, your highscore is {currentHighscore}";
        }
            
        CoinManager.Instance.profit = 0;
    }

    // Alles wat 's nachts gebeurt, in volgorde. Pas als alles verwerkt is wordt er opgeslagen,
    // zodat de save nooit halverwege een dag staat.
    private IEnumerator NightRoutine(bool rain, bool storm)
    {
        // 0.5s: gereedschap terug, machines + kippen, planten groeien
        yield return new WaitForSeconds(0.5f);

        ReturnHeldCompost();

        SeedSelectionUI.ActiveSelectedPlant = null;
        SeedSelectionUI.ActiveSelectedTool = null;
        SeedSelectionUI.Instance.ReturnWateringCan();
        SeedSelectionUI.Instance.ReturnScythe();

        FindAnyObjectByType<Composter>()?.ProcessNewDay();
        ProcessFoodProcessor();
        ProcessChickens();

        UpdateUI();

        foreach (var plot in allPlots)
        {
            plot.AdvanceDay();
        }

        // 3s: weer
        yield return new WaitForSeconds(2.5f);

        if (rain)
        {
            foreach (var plot in allPlots)
            {
                plot.WaterPlant();
            }
        }

        if (storm)
        {
            ApplyStorm();
        }

        // 3.5s: opgegeten gewassen
        yield return new WaitForSeconds(0.5f);
        if (anyPlantsEaten == true)
        {
            NotificationManager.Instance.ShowNotification("You left your crops unharvested, the wild animals ate them!", 3f);
            ShopManager.UnlockSeed("grape");
        }
        anyPlantsEaten = false;

        // 4s: meldingen over het weer
        yield return new WaitForSeconds(0.5f);

        if (rain)
        {
            NotificationManager.Instance.ShowNotification("Nice, it rained last night!");
        }

        if (storm)
        {
            NotificationManager.Instance.ShowNotification("Oh no! It stormed last night!");
            ShopManager.UnlockSeed("corn");

            if (PlayerPrefs.GetInt("stormTalisman_available", 0) == 0)
            {
                PlayerPrefs.SetInt("stormTalisman_available", 1);
                NotificationManager.Instance.ShowNotification("A new talisman is available in the store", 3f);
            }
        }

        // Nacht klaar → opslaan
        SavePlotStates();
        isEndingDay = false;
        SaveManager.SaveGame();
    }

    private void RecalculateStormChance()
    {
        stormChancePercent = stormChanceBasePercent; // basis kans

        foreach (var plot in allPlots)
        {
            var plant = plot.GetPlantedPlant();
            if (plant != null && plant.seedType == "corn" && plot.dead == false)
            {
                stormChancePercent += 1;
            }
        }

        Debug.Log("Storm chance recalculated: " + stormChancePercent + "%");
    }

    private void ApplyStorm()
    {
        foreach (var plot in allPlots)
        {
            if (plot.growthStage > 0)
            {
                if (Random.value <= 0.5f) // 50% kans
                {
                    plot.dead = true;
                    plot.UpdateSprite();
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (dayButtonText != null)
            dayButtonText.text = $"End Day {currentDay}";
    }

    public void SaveComposterState(Composter comp)
    {
        if (comp == null) return;

        composterState = new ComposterState
        {
            isFull = comp.isFull,
            isReady = comp.isReady,
            isTrashcan = comp.isTrashcan
        };
    }

    // Compost in de hand gaat terug in de composter, anders raakt hij kwijt
    // (bij scenewissel, einde dag of ander gereedschap pakken)
    public void ReturnHeldCompost()
    {
        if (SeedSelectionUI.ActiveSelectedTool != "compost") return;

        SeedSelectionUI.ActiveSelectedTool = null;

        composterState.isFull = true;
        composterState.isReady = true;

        var comp = FindFirstObjectByType<Composter>();
        if (comp != null) RestoreComposterState(comp);
    }

    public void RestoreComposterState(Composter comp)
    {
        if (comp == null) return;

        comp.isFull = composterState.isFull;
        comp.isReady = composterState.isReady;
        comp.UpdateVisual();
    }

    private void ProcessFoodProcessor()
    {
        bool full = PlayerPrefs.GetInt("Processor_full", 0) == 1;

        if (full)
        {
            PlayerPrefs.SetInt("Processor_done", 1);
            PlayerPrefs.SetInt("Processor_full", 0);
            PlayerPrefs.Save();
        }
    }

    private void ProcessChickens()
    {
        Chicken.ProcessChickenDay(1);
        Chicken.ProcessChickenDay(2);
    }

    private void CheckPlantBook()
    {
        if (PlayerPrefs.GetInt("plantbook_available", 0) == 0 && unlockedPlants == 6)
        {
            PlayerPrefs.SetInt("plantbook_available", 1);
            NotificationManager.Instance.ShowNotification("You discovered all the seeds, a new item can be found in the store!");

        }
    }

    // ================= SAVE / LOAD (via SaveManager) =================

    public void SaveTo(SaveData data)
    {
        // Als we op de farm zijn eerst de actuele stand ophalen, anders geldt de laatst bewaarde stand
        SavePlotStates();
        SaveComposterState(FindFirstObjectByType<Composter>());

        data.currentDay = currentDay;
        data.unlockedPlants = unlockedPlants;
        data.taskLeft = taskLeft;

        data.rainChanceBasePercent = rainChancePercent;
        data.stormChanceBasePercent = stormChanceBasePercent;
        data.shinyChanceBasePercent = shinyChanceBasePercent;

        data.plots = new List<PlotSaveData>(plotStates.Values);

        data.composterIsFull = composterState.isFull;
        data.composterIsReady = composterState.isReady;
        data.composterIsTrashcan = composterState.isTrashcan;
    }

    public void LoadFrom(SaveData data)
    {
        currentDay = data.currentDay;
        unlockedPlants = data.unlockedPlants;
        taskLeft = data.taskLeft;

        rainChancePercent = data.rainChanceBasePercent;
        stormChanceBasePercent = data.stormChanceBasePercent;
        shinyChanceBasePercent = data.shinyChanceBasePercent;

        plotStates.Clear();
        if (data.plots != null)
        {
            foreach (var p in data.plots)
            {
                plotStates[p.key] = p;
            }
        }

        composterState.isFull = data.composterIsFull;
        composterState.isReady = data.composterIsReady;
        composterState.isTrashcan = data.composterIsTrashcan;
    }


}
