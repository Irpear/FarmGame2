using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class RainTalisman : MonoBehaviour
{
    public int maxLevel = 10;
    public int baseCost = 100;
    public int costIncrease = 100;

    public float increasePercent = 5f; // +5% regen per level

    public TextMeshProUGUI costText;
    public TextMeshProUGUI percentageInfoText;
    public Sprite[] levelSprites;

    private Button btn;
    private UnityEngine.UI.Image img;

    private int Level
    {
        get => PlayerPrefs.GetInt("RainUpgradeLevel", 0);
        set { PlayerPrefs.SetInt("RainUpgradeLevel", value); PlayerPrefs.Save(); }
    }

    private void Start()
    {
        btn = GetComponent<Button>();
        img = GetComponent<UnityEngine.UI.Image>();
        UpdateUI();
    }

    public void BuyUpgrade()
    {
        int cost = GetCurrentCost();

        if (CoinManager.Instance.coins < cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }

        // pay
        CoinManager.Instance.AddCoins(-cost);

        // level up
        Level++;

        // upgrade effect
        ApplyRainIncrease();

        // update visuals
        UpdateUI();
    }

    private int GetCurrentCost()
    {
        return baseCost + (Level * costIncrease);
    }

    private void ApplyRainIncrease()
    {
        DayManager.Instance.rainChancePercent += increasePercent;

        // cap at 100%
        DayManager.Instance.rainChancePercent = Mathf.Min(100f, DayManager.Instance.rainChancePercent);
    }

    private void UpdateUI()
    {
        if (Level >= maxLevel)
        {
            costText.text = "MAX";
            percentageInfoText.text = $"Rain chance = 50%";
            btn.interactable = false;
        }
        else
        {
            int cost = GetCurrentCost();
            float current = DayManager.Instance.rainChancePercent;
            float next = current + increasePercent;

            costText.text = $"{cost}";
            percentageInfoText.text = $"+{increasePercent}% chance to rain ({current}% -> {next}%)";
        }

        // sprite update
        if (img != null && levelSprites.Length > 0)
            img.sprite = levelSprites[Mathf.Min(Level, levelSprites.Length - 1)];
    }
}
