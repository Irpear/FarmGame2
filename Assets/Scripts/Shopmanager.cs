using TMPro;
using UnityEngine;

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

    void Start()
    {
        UpdatePriceTexts();
    }

    public void UpdatePriceTexts()
    {
        if (carrotPriceText != null) carrotPriceText.text = carrotPrice.ToString();
        if (tomatoPriceText != null) tomatoPriceText.text = tomatoPrice.ToString();
        if (wheatPriceText != null) wheatPriceText.text = wheatPrice.ToString();
        if (cornPriceText != null) cornPriceText.text = cornPrice.ToString();
        if (grapePriceText != null) grapePriceText.text = grapePrice.ToString();
        if (potatoPriceText != null) potatoPriceText.text = potatoPrice.ToString();
    }

    public void BuySeed(string seedType, int price)
    {
        if (CoinManager.Instance.coins >= price)
        {
            CoinManager.Instance.AddCoins(-price);
            SeedManager.Instance.AddSeeds(seedType, 1);
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