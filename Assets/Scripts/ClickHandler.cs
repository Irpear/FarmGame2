using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ClickHandler : MonoBehaviour
{

    public GameObject seedsPanel;
    public GameObject upgradesPanel;
    public GameObject talismanPanel;
    public GameObject animalPanel;


    public void LoadStore()
    {
        DayManager.Instance.SavePlotStates();
        SceneTransition.Instance.LoadSceneWithFade("ShopScene");
    }

    public void LoadBarn()
    {
        DayManager.Instance.SavePlotStates();
        SceneTransition.Instance.LoadSceneWithFade("BarnScene");
    }

    public void LoadFarm()
    {
        DayManager.Instance.SavePlotStates();
        SceneTransition.Instance.LoadSceneWithFade("FarmScene");

    }

    public void LoadMinigames()
    {
        SceneTransition.Instance.LoadSceneWithFade("MinigameScene");
    }

    public void ShowSeeds()
    {
        SceneTransition.Instance.SwitchPanels(() => {
            seedsPanel.SetActive(true);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(false);
        });
    }

    public void ShowUpgrades()
    {
        SceneTransition.Instance.SwitchPanels(() => {
            seedsPanel.SetActive(false);
            upgradesPanel.SetActive(true);
            talismanPanel.SetActive(false);
            animalPanel.SetActive(false);
        });
    }

    public void ShowTalisman()
    {
        SceneTransition.Instance.SwitchPanels(() => {
            seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(true);
        animalPanel.SetActive(false);
        });
    }

    public void ShowAnimals()
    {
        SceneTransition.Instance.SwitchPanels(() => {
            seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(true);
        });
    }

    public void ShowShopHub()
    {
        SceneTransition.Instance.SwitchPanels(() => {
            seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(false);
        });
    }



    public void RESETALLPROGRESS()
    {
        SaveManager.DeleteSave();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Deze managers blijven tussen scenes bestaan, weghalen zodat ze bij de herstart vers beginnen
        if (DayManager.Instance != null) Destroy(DayManager.Instance.gameObject);
        if (CoinManager.Instance != null) Destroy(CoinManager.Instance.gameObject);
        if (SeedManager.Instance != null) Destroy(SeedManager.Instance.gameObject);

        SceneManager.LoadScene(0); // Herstart game
    }
}
