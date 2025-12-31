using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    public int composterCost = 400;
    public Button buyComposterButton;

    public TextMeshProUGUI costText;
    public TextMeshProUGUI infoText;
    public Image coinImage;


    void Awake()
    {
        UpdateUI();
    }

    public void BuyComposterUpgrade()
    {

        if (CoinManager.Instance.coins < composterCost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // pay
        CoinManager.Instance.AddCoins(-composterCost);
        PlayerPrefs.SetInt("composter_unlocked", 1);
        PlayerPrefs.Save();

        UpdateUI();

        NotificationManager.Instance.ShowNotification("Composter unlocked!");
        Debug.Log("Composter purchased!");
    }

    private void UpdateUI()
    {

        if (PlayerPrefs.GetInt("composter_unlocked", 0) == 1)
        {
            buyComposterButton.interactable = false;
            costText.gameObject.SetActive(false);
            infoText.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
        }
    }
}
