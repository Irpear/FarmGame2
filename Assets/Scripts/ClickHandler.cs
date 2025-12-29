using UnityEngine;
using UnityEngine.SceneManagement;


public class ClickHandler : MonoBehaviour
{

    public GameObject seedsPanel;
    public GameObject upgradesPanel;
    public GameObject talismanPanel;


    public void LoadStore()
    {
        DayManager.Instance.SavePlotStates();
        SceneManager.LoadScene("ShopScene");
    }

    public void LoadBarn()
    {
        DayManager.Instance.SavePlotStates();
        SceneManager.LoadScene("BarnScene");
    }

    public void LoadFarm()
    {
        DayManager.Instance.SavePlotStates();
        SceneManager.LoadScene("FarmScene");
    }

    public void ShowSeeds()
    {
        seedsPanel.SetActive(true);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(true);
        talismanPanel.SetActive(false);
    }

    public void ShowTalisman()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(true);
    }

    public void ShowShopHub()
    {
        seedsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        talismanPanel.SetActive(false);
    }


}
