using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public int currentDay = 1;
    public TextMeshProUGUI dayButtonText;

    //// Event: scripts kunnen hiernaar luisteren
    //public event Action OnDayEnded;

    public NightTransition nightTransition;

    public Plot[] allPlots;

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


        UpdateUI();
}

    public void EndDay()
    {
        Debug.Log("Day " + currentDay + " ended.");

        currentDay++;

        StartCoroutine(DelayedUIUpdate());

        nightTransition.PlayTransition(() => {
            
            Debug.Log("EndDay called. Now day is: " + currentDay);

            foreach (var plot in allPlots)
            {
                plot.AdvanceDay();
            }
        });

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
