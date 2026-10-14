using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StormTalisman : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 1000;                 // vaste prijs
    public float increasePercent = -1f;     // hoeveel % storm extra

    [Header("UI References")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI percentageInfoText;
    public Sprite purchasedSprite;         // sprite na koop
    public Sprite defaultSprite;           // sprite voor koop
    public Image coinImage;

    private Button btn;
    private Image img;

    private const string PREF_KEY = "StormTalismanPurchased";

    private bool Purchased
    {
        get => PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_KEY, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    // Awake i.p.v. Start: anders is de talisman bij het openen van het panel 1 frame ontgrendeld te zien
    private void Awake()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();

        UpdateUI();
    }

    public void BuyTalisman()
    {
        if (Purchased)
            return;

        if (CoinManager.Instance.coins < cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // Pay
        CoinManager.Instance.AddCoins(-cost);

        // Apply effect once
        DayManager.Instance.stormChanceBasePercent += increasePercent;

        // mark as purchased
        Purchased = true;
        Trophies.CheckBuyEverything();

        // update visuals
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PlayerPrefs.GetInt("stormTalisman_available", 0) == 0)
        {
            ButtonUtils.SetInteractable(btn, false);
            costText.gameObject.SetActive(false);
            percentageInfoText.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
            return;
        }

        if (Purchased)
        {
            costText.text = "MAX";
            percentageInfoText.text = $"Base storm chance = 1%";
            ButtonUtils.SetInteractable(btn, false);

            if (purchasedSprite != null)
                img.sprite = purchasedSprite;
        }
        else
        {
            float current = DayManager.Instance.stormChanceBasePercent;
            float next = current + increasePercent;

            costText.text = $"{cost}";
            percentageInfoText.text = $"Halve storm chance ({current}% → {next}%)";

            if (defaultSprite != null)
                img.sprite = defaultSprite;
        }
    }
}
