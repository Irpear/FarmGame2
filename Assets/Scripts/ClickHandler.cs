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
        seedsPanel.SetActive(true);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(true);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(false);
    }

    public void ShowTalisman()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(true);
        animalPanel.SetActive(false);
    }

    public void ShowAnimals()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(true);
    }

    public void ShowShopHub()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
        animalPanel.SetActive(false);
    }



    public void RESETALLPROGRESS()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(0); // Herstart game
    }
}
