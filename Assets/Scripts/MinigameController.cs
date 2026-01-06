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

    public int ChosenMinigame;


    private void Start()
    {
        ChosenMinigame = Random.Range(0, 2);
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
    }


    public void StartAccounting()
    {
        AccountingPanel.SetActive(true);
    }
    public void StartPacking()
    {
        PackingPanel.SetActive(true);
    }
}
