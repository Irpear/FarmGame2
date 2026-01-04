using UnityEngine;

public class AnimalManager : MonoBehaviour
{

    public int chicken1Cost = 100;
    public int chicken2Cost = 1000;

    public void BuyChicken1()
    {
        if (CoinManager.Instance.coins < chicken1Cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }
        Chicken.UnlockChicken(1);

    }

    public void BuyChicken2()
    {
        if (CoinManager.Instance.coins < chicken2Cost)
        {
            NotificationManager.Instance.ShowNotification("Not enough coins!");
            return;
        }
        Chicken.UnlockChicken(2);

    }
}
