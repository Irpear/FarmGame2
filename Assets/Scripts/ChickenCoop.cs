using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChickenCoop : MonoBehaviour
{

    public void ClickCoop()
    {
        if (PlayerPrefs.GetInt("scythe_available", 0) == 0)
        PlayerPrefs.SetInt("scythe_available", 1); 
        PlayerPrefs.Save();
        NotificationManager.Instance.ShowNotification("More items have been unlocked at the store");
    }

}
