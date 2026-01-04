using UnityEngine;
using UnityEngine.UI;

public class AnimalManager : MonoBehaviour
{

    public int chicken1Cost = 100;
    public int chicken2Cost = 1000;

    public Button buyChicken1Button;
    public Button buyChicken2Button;
    public GameObject costTextChicken1;
    public GameObject costTextChicken2;
    public GameObject infoTextChicken1;
    public GameObject infoTextChicken2;
    public GameObject coinImageChicken1;
    public GameObject coinImageChicken2;


    void Awake()
    {
        UpdateUI();
    }


    public void BuyChicken1()
    {
        if (CoinManager.Instance.coins < chicken1Cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }
        Chicken.UnlockChicken(1);
        UpdateUI();

    }

    public void BuyChicken2()
    {
        if (CoinManager.Instance.coins < chicken2Cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }
        Chicken.UnlockChicken(2);
        UpdateUI();

    }


    private void UpdateUI()
    {

        if (PlayerPrefs.GetInt("chicken1_unlocked", 0) == 1 || PlayerPrefs.GetInt("chicken1_available", 0) == 0)
        {
            buyChicken1Button.interactable = false;
            costTextChicken1.gameObject.SetActive(false);
            infoTextChicken1.gameObject.SetActive(false);
            coinImageChicken1.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("chicken2_unlocked", 0) == 1 || PlayerPrefs.GetInt("chicken2_available", 0) == 0)
        {
            buyChicken2Button.interactable = false;
            costTextChicken2.gameObject.SetActive(false);
            infoTextChicken2.gameObject.SetActive(false);
            coinImageChicken2.gameObject.SetActive(false);
        }
    }
}
