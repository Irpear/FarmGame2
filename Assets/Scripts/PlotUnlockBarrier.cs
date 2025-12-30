using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlotUnlockBarrier : MonoBehaviour
{
    public string barrierID = "plotBarrier_x";
    public int unlockCost = 50;
    public TextMeshPro costText;
    private GameObject forceCompostScreen;
    private GameObject nightOverlay;
    private Image nightOverlayImage;

    private void Start()
    {
        costText.text = unlockCost.ToString();

        nightOverlay = GameObject.Find("NightOverlay");
        if (nightOverlay != null)
        {
            nightOverlayImage = nightOverlay.GetComponent<Image>();
        }

        forceCompostScreen = GameObject.Find("ForceCompostScreen");

        if (PlayerPrefs.GetInt(barrierID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        
        //if (EventSystem.current.IsPointerOverGameObject()) return;

        // UI checks die erboven kliks moeten blokkeren
        if (NotificationManager.Instance.IsShowing()) return;
        if (nightOverlayImage != null && nightOverlayImage.raycastTarget) return;
        if (forceCompostScreen != null && forceCompostScreen.activeSelf) return;

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
