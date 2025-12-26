using UnityEngine;
using UnityEngine.SceneManagement;


public class ClickHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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


}
