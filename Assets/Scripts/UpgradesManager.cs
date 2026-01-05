using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    public int composterCost = 400;
    public int scytheCost = 100;
    public int processorCost = 400;
    public int feeder2Cost = 200;

    public Button buyComposterButton;
    public Button buyScytheButton;
    public Button buyProcessorButton;
    public Button buyFeeder2Button;

    public TextMeshProUGUI costTextComposter;
    public TextMeshProUGUI infoTextComposter;
    public Image coinImageComposter;

    public TextMeshProUGUI costTextScythe;
    public TextMeshProUGUI infoTextScythe;
    public Image coinImageScythe;

    public TextMeshProUGUI costTextProcessor;
    public TextMeshProUGUI infoTextProcessor;
    public Image coinImageProcessor;

    public TextMeshProUGUI costTextFeeder2;
    public TextMeshProUGUI infoTextFeeder2;
    public Image coinImageFeeder2;


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

    public void BuyProcessor()
    {

        if (CoinManager.Instance.coins < processorCost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // pay
        CoinManager.Instance.AddCoins(-processorCost);
        PlayerPrefs.SetInt("processor_unlocked", 1);
        PlayerPrefs.Save();

        UpdateUI();

        NotificationManager.Instance.ShowNotification("Food Processor unlocked!");
    }

    public void BuyFeeder2()
    {

        if (CoinManager.Instance.coins < feeder2Cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // pay
        CoinManager.Instance.AddCoins(-feeder2Cost);
        PlayerPrefs.SetInt("feeder2_unlocked", 1);
        PlayerPrefs.SetInt("chicken2_available", 1);
        PlayerPrefs.Save();

        UpdateUI();

        NotificationManager.Instance.ShowNotification("A second feeder unlocked!");
        NotificationManager.Instance.ShowNotification("More items have been unlocked at the store");
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

        if (PlayerPrefs.GetInt("processor_unlocked", 0) == 1 || PlayerPrefs.GetInt("processor_available", 0) == 0)
        {
            buyProcessorButton.interactable = false;
            costTextProcessor.gameObject.SetActive(false);
            infoTextProcessor.gameObject.SetActive(false);
            coinImageProcessor.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("feeder2_unlocked", 0) == 1 || PlayerPrefs.GetInt("feeder2_available", 0) == 0)
        {
            buyFeeder2Button.interactable = false;
            costTextFeeder2.gameObject.SetActive(false);
            infoTextFeeder2.gameObject.SetActive(false);
            coinImageFeeder2.gameObject.SetActive(false);
        }
    }
}
