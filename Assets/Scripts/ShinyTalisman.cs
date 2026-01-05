using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShinyTalisman : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 1000;                 // vaste prijs
    public float increasePercent = 1f;     // hoeveel % shiny extra

    [Header("UI References")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI percentageInfoText;
    public Sprite purchasedSprite;         // sprite na koop
    public Sprite defaultSprite;           // sprite voor koop

    private Button btn;
    private Image img;

    private const string PREF_KEY = "ShinyTalismanPurchased";

    private bool Purchased
    {
        get => PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_KEY, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    private void Start()
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
        DayManager.Instance.shinyChancePercent += increasePercent;

        // mark as purchased
        Purchased = true;

        // update visuals
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (Purchased)
        {
            costText.text = "MAX";
            percentageInfoText.text = $"Base shiny chance = 2%";
            btn.interactable = false;

            if (purchasedSprite != null)
                img.sprite = purchasedSprite;
        }
        else
        {
            float current = DayManager.Instance.shinyChancePercent;
            float next = current + increasePercent;

            costText.text = $"{cost}";
            percentageInfoText.text = $"Double shiny chance ({current}% → {next}%)";

            if (defaultSprite != null)
                img.sprite = defaultSprite;
        }
    }
}
