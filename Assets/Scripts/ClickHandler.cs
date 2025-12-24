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
        SceneManager.LoadScene("StoreScene");
    }

    public void LoadBarn()
    {
        SceneManager.LoadScene("BarnScene");
    }

    public void LoadFarm()
    {
        SceneManager.LoadScene("FarmScene");
    }

}
