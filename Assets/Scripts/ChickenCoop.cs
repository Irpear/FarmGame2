using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ChickenCoop : MonoBehaviour
{

    public Image ChickenCoopImage;

    public Sprite ChickenCoopLockedSprite;
    public Sprite ChickenCoopSprite;

    public GameObject ChickenCoopPanel;
    public GameObject foodSelectionPanel;

    public int coopCost = 200;


    void Awake()
    {
        UpdateUI();
    }

    public void ClickCoop()
    {
        if (PlayerPrefs.GetInt("coop_unlocked", 0) == 1) ShowChickenCoop();
        else
        {
            if (CoinManager.Instance.coins < coopCost)
            {
                NotificationManager.Instance.ShowNotification($"You need {coopCost} coins!");
                return;
            }

            // pay
            CoinManager.Instance.AddCoins(-coopCost);
            PlayerPrefs.SetInt("coop_unlocked", 1);
            PlayerPrefs.SetInt("scythe_available", 1);
            PlayerPrefs.Save();
            NotificationManager.Instance.ShowNotification("Chicken coop unlocked!");
            NotificationManager.Instance.ShowNotification("More items have been unlocked at the store");

            UpdateUI();
        }
    }


    private void UpdateUI()
    {
        if (PlayerPrefs.GetInt("coop_unlocked", 0) == 1) ChickenCoopImage.sprite = ChickenCoopSprite;
        else ChickenCoopImage.sprite = ChickenCoopLockedSprite;
    }

    public void ShowChickenCoop()
    {
        ChickenCoopPanel.SetActive(true);
        foodSelectionPanel.SetActive(false);
    }

    public void ShowBarn()
    {
        ChickenCoopPanel.SetActive(false);
    }

}
