using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShopManager : MonoBehaviour
{
    [Header("Seed Prices")]
    public int carrotPrice = 1;
    public int tomatoPrice = 2;
    public int wheatPrice = 1;
    public int cornPrice = 3;
    public int grapePrice = 4;
    public int potatoPrice = 2;

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

    void Start()
    {
        UpdateTexts();
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