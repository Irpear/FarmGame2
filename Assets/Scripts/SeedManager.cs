using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    public int seeds = 6;

    private void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void AddSeeds(int amount)
    {
        seeds += amount;
    }
}