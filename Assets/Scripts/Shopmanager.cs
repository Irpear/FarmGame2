using System.Collections.Generic;
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

    [Header("Stock Text Fields")]
    public TextMeshProUGUI carrotStock;
    public TextMeshProUGUI tomatoStock;
    public TextMeshProUGUI wheatStock;
    public TextMeshProUGUI cornStock;
    public TextMeshProUGUI grapeStock;
    public TextMeshProUGUI potatoStock;

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

        UpdateStockTexts();
    }

    private void UpdateStockTexts()
    {
        if (carrotStock != null) carrotStock.text = GetStock("carrot").ToString();
        if (tomatoStock != null) tomatoStock.text = GetStock("tomato").ToString();
        if (wheatStock != null) wheatStock.text = GetStock("wheat").ToString();
        if (cornStock != null) cornStock.text = GetStock("corn").ToString();
        if (grapeStock != null) grapeStock.text = GetStock("grape").ToString();
        if (potatoStock != null) potatoStock.text = GetStock("potato").ToString();
    }

    private int GetStock(string seedType)
    {
        return PlayerPrefs.GetInt($"shop_stock_{seedType}", 0);
    }

    private void SetStock(string seedType, int amount)
    {
        PlayerPrefs.SetInt($"shop_stock_{seedType}", amount);
        PlayerPrefs.Save();
    }

    public void BuySeed(string seedType, int price)
    {
        int stock = GetStock(seedType);

        if (stock <= 0)
        {
            // hier miss plaatje rood maken
            Debug.Log($"No {seedType} stock left!");
            return;
        }

        if (CoinManager.Instance.coins >= price)
        {
            CoinManager.Instance.AddCoins(-price);
            SeedManager.Instance.AddSeeds(seedType, 1);
            SetStock(seedType, stock - 1);
            UpdateTexts();
            Debug.Log($"Bought 1 {seedType} seed for {price} coins. Stock left: {stock - 1}");
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


    public static void ResetDailyStock()
    {
        int totalPlots = PlayerPrefs.GetInt("totalUnlockedPlots", 6);

        // Verzamel unlocked seeds
        List<string> unlockedSeeds = new List<string>();
        string[] allSeeds = { "carrot", "tomato", "wheat", "corn", "grape", "potato" };

        foreach (string seed in allSeeds)
        {
            if (IsSeedUnlocked(seed))
            {
                unlockedSeeds.Add(seed);
            }
        }

        if (unlockedSeeds.Count == 0)
        {
            Debug.LogWarning("No seeds unlocked!");
            return;
        }

        // Reset alle stocks naar 0
        foreach (string seed in allSeeds)
        {
            PlayerPrefs.SetInt($"shop_stock_{seed}", 0);
        }

        //// Verdeel stock: minstens 1 per unlocked seed
        //int remaining = totalPlots - unlockedSeeds.Count;

        //// Geef iedereen eerst 1
        //foreach (string seed in unlockedSeeds)
        //{
        //    PlayerPrefs.SetInt($"shop_stock_{seed}", 1);
        //}

        // Verdeel de rest random
        while (totalPlots > 0)
        {
            string randomSeed = unlockedSeeds[Random.Range(0, unlockedSeeds.Count)];
            int current = PlayerPrefs.GetInt($"shop_stock_{randomSeed}", 0);
            PlayerPrefs.SetInt($"shop_stock_{randomSeed}", current + 1);
            totalPlots--;
        }

        PlayerPrefs.Save();

        Debug.Log($"Daily stock reset! Total plots: {totalPlots}, Unlocked seeds: {unlockedSeeds.Count}");
    }
}