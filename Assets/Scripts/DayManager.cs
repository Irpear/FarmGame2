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

    //// Event: scripts kunnen hiernaar luisteren
    //public event Action OnDayEnded;

    public NightTransition nightTransition;

    public Plot[] allPlots;

    private Dictionary<string, int> plotStates = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    // Zoek de UI
    if (dayButtonText == null)
        dayButtonText = GameObject.Find("DayButtonText")?.GetComponent<TextMeshProUGUI>();

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
                    plotStates[key] = plot.growthStage;
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
                    plot.growthStage = plotStates[key];
                    plot.UpdateSprite(); // Moet public worden in Plot script!
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

        StartCoroutine(DelayedUIUpdate());

        nightTransition.PlayTransition(() => {
            
            Debug.Log("EndDay called. Now day is: " + currentDay);

        });

        StartCoroutine(DelayedGrowth());

    }

    private IEnumerator DelayedGrowth()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var plot in allPlots)
        {
            plot.AdvanceDay();
        }

        SavePlotStates();
    }

    private IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (dayButtonText != null)
            dayButtonText.text = $"End Day {currentDay}";
    }
}
