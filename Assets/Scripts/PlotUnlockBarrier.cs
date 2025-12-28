using UnityEngine;
using TMPro;

public class PlotUnlockBarrier : MonoBehaviour
{
    public string barrierID = "plotBarrier_x";
    public int unlockCost = 50;
    public TextMeshPro costText;

    private void Start()
    {
        costText.text = unlockCost.ToString();

        if (PlayerPrefs.GetInt(barrierID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {

        if (NotificationManager.Instance.IsShowing()) return;

        if (CoinManager.Instance.coins >= unlockCost)
        {
            CoinManager.Instance.AddCoins(-unlockCost);

            PlayerPrefs.SetInt(barrierID, 1);
            PlayerPrefs.Save();

            // geluid/particle hier eventueel
            Destroy(gameObject); // maakt plot eronder vrij
        }
        else
        {
            NotificationManager.Instance.ShowNotification("You don't have enough coins!");
        }
    }
}
