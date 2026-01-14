using TMPro;
using UnityEngine;

public class MinigameController : MonoBehaviour
{

    public GameObject AccountingPanel;
    public GameObject AccountingStartPanel;
    public TextMeshProUGUI AccountingStreakTime;

    public GameObject PackingPanel;
    public GameObject PackingStartPanel;
    public TextMeshProUGUI PackingStreakSize;

    public GameObject CleaningPanel;
    public GameObject CleaningStartPanel;
    public TextMeshProUGUI CleaningStreakTime;

    public int completedTasks = 0;

    private const string CompletedTasksKey = "CompletedMinigameTasks";

    public int ChosenMinigame;


    private void Start()
    {
        completedTasks = PlayerPrefs.GetInt(CompletedTasksKey, 0);

        ChosenMinigame = Random.Range(0, 3);
        if (ChosenMinigame == 0)
        {
            AccountingStartPanel.SetActive(true);
            int streak = PlayerPrefs.GetInt("AccountingStreak", 0);
            float adjustedTime = 15 - streak;
            AccountingStreakTime.text = adjustedTime.ToString();
        }
        else if (ChosenMinigame == 1)
        {
            PackingStartPanel.SetActive(true);
            int streak = PlayerPrefs.GetInt("PackingStreak", 0);
            float adjustedSize = 5 + streak;
            PackingStreakSize.text = adjustedSize.ToString();
            PackingStartPanel.SetActive(true);
        }
        else if (ChosenMinigame == 2)
        {
            CleaningStartPanel.SetActive(true);
            int streak = PlayerPrefs.GetInt("CleaningStreak", 0);
            float adjustedTime = 10 - streak;
            CleaningStreakTime.text = adjustedTime.ToString();
        }
    }


    public void StartAccounting()
    {
        AccountingPanel.SetActive(true);
    }
    public void StartPacking()
    {
        PackingPanel.SetActive(true);
    }
    public void StartCleaning()
    {
        CleaningPanel.SetActive(true);
    }

    public void TaskCompleted()
    {
        completedTasks++;

        PlayerPrefs.SetInt(CompletedTasksKey, completedTasks);
        PlayerPrefs.Save();

        if (completedTasks == 15)
        {
            PlayerPrefs.SetInt("shinyTalisman_available", 1);
            PlayerPrefs.Save();
            NotificationManager.Instance.ShowNotification("A new talisman is available in the store", 3f);
        }
    }
}
