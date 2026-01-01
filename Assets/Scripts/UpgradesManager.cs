using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    public int composterCost = 400;
    public int scytheCost = 400;

    public Button buyComposterButton;
    public Button buyScytheButton;

    public TextMeshProUGUI costTextComposter;
    public TextMeshProUGUI infoTextComposter;
    public Image coinImageComposter;

    public TextMeshProUGUI costTextScythe;
    public TextMeshProUGUI infoTextScythe;
    public Image coinImageScythe;


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
        ShopManager.UnlockSeed("potato");
    }

    public void BuyScytheUpgrade()
    {

        if (CoinManager.Instance.coins < scytheCost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // pay
        CoinManager.Instance.AddCoins(-scytheCost);
        PlayerPrefs.SetInt("scythe_unlocked", 1);
        PlayerPrefs.Save();

        UpdateUI();

        NotificationManager.Instance.ShowNotification("Scythe unlocked!");
    }

    private void UpdateUI()
    {

        if (PlayerPrefs.GetInt("composter_unlocked", 0) == 1)
        {
            buyComposterButton.interactable = false;
            costTextComposter.gameObject.SetActive(false);
            infoTextComposter.gameObject.SetActive(false);
            coinImageComposter.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("scythe_unlocked", 0) == 1 || PlayerPrefs.GetInt("scythe_available", 0) == 0)
        {
            buyScytheButton.interactable = false;
            costTextScythe.gameObject.SetActive(false);
            infoTextScythe.gameObject.SetActive(false);
            coinImageScythe.gameObject.SetActive(false);
        }
    }
}
