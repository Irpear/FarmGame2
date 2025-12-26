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
        SceneManager.LoadScene("StoreScene");
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

    public void BuySeeds()
    {
        if (CoinManager.Instance.coins > 0)
        {
            CoinManager.Instance.AddCoins(-1);
            SeedManager.Instance.AddSeeds(1);
            //Debug.Log($"You have {SeedManager.Instance.seeds} seeds");
        }
        else
        {
            Debug.Log($"You have no money.");
        }
    }
}
