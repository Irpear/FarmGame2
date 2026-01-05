using TMPro;
using UnityEngine;

public class MinigameController : MonoBehaviour
{

    public GameObject AccountingPanel;
    public TextMeshProUGUI AccountingStreakTime;


    private void Start()
    {
        int streak = PlayerPrefs.GetInt("AccountingStreak", 0);
        float adjustedTime = 15 - streak;
        AccountingStreakTime.text = adjustedTime.ToString();
    }


    public void StartAccounting()
    {
        AccountingPanel.SetActive(true);
    }
}
