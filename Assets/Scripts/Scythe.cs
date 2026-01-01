using UnityEngine;
using UnityEngine.UI;

public class Scythe : MonoBehaviour
{
    public Button ScytheButton;


    private const string PREF_KEY = "scythe_unlocked";

    private bool Purchased
    {
        get => PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_KEY, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    void Awake()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (Purchased) ScytheButton.interactable = true;
        else ScytheButton.interactable = false;
    }
}
