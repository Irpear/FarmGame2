using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Seed Prices")]
    public int carrotPrice = 2;
    public int tomatoPrice = 3;
    public int wheatPrice = 4;
    public int cornPrice = 5;
    public int grapePrice = 3;
    public int potatoPrice = 3;

    [Header("Price Text Fields")]
    public TextMeshProUGUI carrotPriceText;
    public TextMeshProUGUI tomatoPriceText;
    public TextMeshProUGUI wheatPriceText;
    public TextMeshProUGUI cornPriceText;
    public TextMeshProUGUI grapePriceText;
    public TextMeshProUGUI potatoPriceText;

    [Header("Inventory Text Fields")]
    public TextMeshProUGUI carrotInventory;
    public TextMeshProUGUI tomatoInventory;
    public TextMeshProUGUI wheatInventory;
    public TextMeshProUGUI cornInventory;
    public TextMeshProUGUI grapeInventory;
    public TextMeshProUGUI potatoInventory;

    [Header("Seed Buttons")]
    public Button carrotButton;
    public Button tomatoButton;
    public Button wheatButton;
    public Button cornButton;
    public Button grapeButton;
    public Button potatoButton;

    [Header("Shop Buttons")]
    public Button animalShopButton;
    public Button minigameButton;

    void Start()
    {
        CheckUnlocks();
        UpdateTexts();

        var minigameButton = GameObject.Find("MinigameButton")?.GetComponent<Button>();

        if (minigameButton == null)
        {
            Debug.LogError("MinigameButton not found!");
            return;
        }

        minigameButton.interactable = DayManager.Instance.taskLeft;
    }

    public void CheckUnlocks()
    {
        // Carrot is altijd unlocked (starter seed)
        SetSeedUnlocked("carrot", true);

    // Check andere seeds
        SetButtonState(tomatoButton, IsSeedUnlocked("tomato"));
        SetButtonState(wheatButton, IsSeedUnlocked("wheat"));
        SetButtonState(cornButton, IsSeedUnlocked("corn"));
        SetButtonState(grapeButton, IsSeedUnlocked("grape"));
        SetButtonState(potatoButton, IsSeedUnlocked("potato"));

        SetButtonState(animalShopButton, (PlayerPrefs.GetInt("animal_shop_unlocked", 0) == 1));
        SetButtonState(minigameButton, (DayManager.Instance.taskLeft));
    }

    private void SetButtonState(Button button, bool unlocked)
    {
        if (button != null)
        {
            button.interactable = unlocked;

            foreach (Transform child in button.transform)
            {
                child.gameObject.SetActive(unlocked);
            }
        }
    }

    public static bool IsSeedUnlocked(string seedType)
    {
        return PlayerPrefs.GetInt($"seed_unlocked_{seedType}", 0) == 1;
    }

    public static void UnlockSeed(string seedType)
    {
        if (!IsSeedUnlocked(seedType))
        {
            PlayerPrefs.SetInt($"seed_unlocked_{seedType}", 1);
            PlayerPrefs.Save();
            Debug.Log($"{seedType} unlocked!");

            // Optioneel: toon notification
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"{seedType} seed unlocked!");
            }

            DayManager.Instance.unlockedPlants++;
        }
    }

    private static void SetSeedUnlocked(string seedType, bool unlocked)
    {
        PlayerPrefs.SetInt($"seed_unlocked_{seedType}", unlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void UpdateTexts()
    {
        if (carrotPriceText != null) { carrotPriceText.text = carrotPrice.ToString(); carrotInventory.text = SeedManager.Instance.GetSeeds("carrot").ToString(); }
        if (tomatoPriceText != null) { tomatoPriceText.text = tomatoPrice.ToString(); tomatoInventory.text = SeedManager.Instance.GetSeeds("tomato").ToString(); }
        if (wheatPriceText != null) { wheatPriceText.text = wheatPrice.ToString(); wheatInventory.text = SeedManager.Instance.GetSeeds("wheat").ToString(); }
        if (cornPriceText != null) { cornPriceText.text = cornPrice.ToString(); cornInventory.text = SeedManager.Instance.GetSeeds("corn").ToString(); }
        if (grapePriceText != null) { grapePriceText.text = grapePrice.ToString(); grapeInventory.text = SeedManager.Instance.GetSeeds("grape").ToString(); }
        if (potatoPriceText != null) { potatoPriceText.text = potatoPrice.ToString(); potatoInventory.text = SeedManager.Instance.GetSeeds("potato").ToString(); }

    }

    public void BuySeed(string seedType, int price)
    {
        if (CoinManager.Instance.coins >= price)
        {
            CoinManager.Instance.AddCoins(-price);
            SeedManager.Instance.AddSeeds(seedType, 1);
            UpdateTexts();
            Debug.Log($"Bought 1 {seedType} seed for {price} coins");
        }
        else
        {
            Debug.Log($"Not enough money. Need {price} coins.");
        }
    }

    public void BuyCarrot() => BuySeed("carrot", carrotPrice);
    public void BuyTomato() => BuySeed("tomato", tomatoPrice);
    public void BuyWheat() => BuySeed("wheat", wheatPrice);
    public void BuyCorn() => BuySeed("corn", cornPrice);
    public void BuyGrape() => BuySeed("grape", grapePrice);
    public void BuyPotato() => BuySeed("potato", potatoPrice);
}