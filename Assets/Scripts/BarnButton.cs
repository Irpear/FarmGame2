using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BarnButton : MonoBehaviour
{

    public Button barnButton;
    public Image barnButtonImage;

    public Sprite barnLockedSprite;
    public Sprite barnSprite;

    public int barnCost = 500;

    private const string PREF_KEY = "barn_unlocked";

    private bool Purchased
    {
        get => PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_KEY, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    void Awake()
    {
        UpdateUI();
    }

    public void ClickBarn()
    {
        if (Purchased) LoadBarn();
        else
        {
            if (CoinManager.Instance.coins < barnCost)
            {
                NotificationManager.Instance.ShowNotification($"You need {barnCost} coins!");
                return;
            }

            // pay
            CoinManager.Instance.AddCoins(-barnCost);
            Purchased = true;

            UpdateUI();

            NotificationManager.Instance.ShowNotification("Barn unlocked!");
            ShopManager.UnlockSeed("wheat");
            NotificationManager.Instance.ShowNotification("More items have been unlocked at the store");
        }
    }

    private void UpdateUI()
    {
        if (Purchased) barnButtonImage.sprite = barnSprite;
        else barnButtonImage.sprite = barnLockedSprite;
    }


    public void LoadBarn()
    {
        DayManager.Instance.SavePlotStates();
        SceneManager.LoadScene("BarnScene");
    }
}
