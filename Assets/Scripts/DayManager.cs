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

    private Dictionary<string, (string plantType, int growthStage, bool isWatered, bool dead, bool composted, bool isShiny, bool isGrape, int grapeMaxHarvests, int grapeHarvestsDone)> plotStates
    = new Dictionary<string, (string, int, bool, bool, bool, bool, bool, int, int)>();

    public float rainChancePercent = 0;
    public float stormChanceBasePercent = 2;
    public float stormChancePercent = 2;

    public bool anyPlantsEaten = false;

    private ComposterState composterState;

    [System.Serializable]
    private struct ComposterState
    {
        public bool isFull;
        public bool isReady;
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
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
        SeedSelectionUI.ActiveSelectedPlant = null;
        SeedSelectionUI.ActiveSelectedTool = null;
        SeedSelectionUI.Instance.ReturnWateringCan();

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

                    plotStates[key] = (plantType, plot.growthStage, plot.isWatered, plot.dead, plot.composted, plot.isShiny, plot.isGrape, plot.grapeMaxHarvests, plot.grapeHarvestsDone);

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

                    plot.chosenVariant = Random.Range(0, 4);

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
        Debug.Log("Day " + currentDay + " ended.");

        currentDay++;

        profitSummary.text = $"You earned {CoinManager.Instance.profit} coins today";
        ResetProfit();

        StartCoroutine(DelayedUIUpdate());

        nightTransition.PlayTransition(() => {
            
            Debug.Log("EndDay called. Now day is: " + currentDay);

        });

        StartCoroutine(DelayedGrowth());

        //events
        if (Random.value <= (rainChancePercent / 100f))
        {
            StartCoroutine(DelayedRain());
        }

        RecalculateStormChance();

        if (Random.value <= (stormChancePercent / 100f))
        {
            StartCoroutine(DelayedStorm());
        }
    }

    private IEnumerator DelayedGrowth()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var plot in allPlots)
        {
            plot.AdvanceDay();
        }

        SavePlotStates();

        yield return new WaitForSeconds(3f);
        if (anyPlantsEaten == true)
        {
            NotificationManager.Instance.ShowNotification("You left your crops unharvested, the wild animals ate them!", 3f);
            ShopManager.UnlockSeed("grape");
        }
        anyPlantsEaten = false;
    }

    private IEnumerator DelayedRain()
    {
        yield return new WaitForSeconds(3f);
        foreach (var plot in allPlots)
        {
            plot.WaterPlant();
        }
        yield return new WaitForSeconds(1f);
        NotificationManager.Instance.ShowNotification("Nice, it rained last night!");
        SavePlotStates();
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

    private IEnumerator DelayedStorm()
    {
        yield return new WaitForSeconds(3f);
        foreach (var plot in allPlots)
        {
            if (plot.growthStage > 0)
            {
                plot.dead = true;
                plot.UpdateSprite();
            }
        }
        yield return new WaitForSeconds(1f);
        NotificationManager.Instance.ShowNotification("Oh no! It stormed last night!");
        yield return new WaitForSeconds(3f);
        ShopManager.UnlockSeed("corn");
        SavePlotStates();
    }

    private IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        SeedSelectionUI.ActiveSelectedPlant = null;
        SeedSelectionUI.ActiveSelectedTool = null;
        SeedSelectionUI.Instance.ReturnWateringCan();

        FindAnyObjectByType<Composter>()?.ProcessNewDay();

        UpdateUI();
    }

    private void ResetProfit()
    {
        CoinManager.Instance.profit = 0;
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
            isReady = comp.isReady
        };
    }

    public void RestoreComposterState(Composter comp)
    {
        if (comp == null) return;

        comp.isFull = composterState.isFull;
        comp.isReady = composterState.isReady;
        comp.UpdateVisual();
    }


}
