using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

//// Seeds toevoegen
//SeedManager.Instance.AddSeeds("carrot", 5);
//SeedManager.Instance.AddSeeds("tomato", 3);

//// Seeds ophalen
//int carrotSeeds = SeedManager.Instance.GetSeeds("carrot");

//// Seeds gebruiken (bijvoorbeeld bij planten)
//if (SeedManager.Instance.UseSeeds("carrot", 1))
//{
//    // Plant carrot
//}

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    // Dictionary om verschillende seed types bij te houden
    private Dictionary<string, int> seeds = new Dictionary<string, int>();

    // Optioneel: start seeds voor verschillende types
    [SerializeField] private int startCarrotSeeds = 6;
    [SerializeField] private int startTomatoSeeds = 6;
    [SerializeField] private int startWheatSeeds = 6;
    [SerializeField] private int startCornSeeds = 6;
    [SerializeField] private int startGrapeSeeds = 6;
    [SerializeField] private int startPotatoSeeds = 6;

    private void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialiseer start seeds
            InitializeSeeds();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeSeeds()
    {
        // Voeg hier seed types toe
        seeds["carrot"] = startCarrotSeeds;
        seeds["tomato"] = startTomatoSeeds;
        seeds["wheat"] = startWheatSeeds;
        seeds["corn"] = startCornSeeds;
        seeds["grape"] = startGrapeSeeds;
        seeds["potato"] = startPotatoSeeds;
    }

    public void AddSeeds(string seedType, int amount)
    {
        if (seeds.ContainsKey(seedType))
        {
            seeds[seedType] += amount;
        }
        else
        {
            seeds[seedType] = amount;
        }
    }

    public int GetSeeds(string seedType)
    {
        if (seeds.ContainsKey(seedType))
        {
            return seeds[seedType];
        }
        return 0;
    }

    public bool UseSeeds(string seedType, int amount = 1)
    {
        if (seeds.ContainsKey(seedType) && seeds[seedType] >= amount)
        {
            seeds[seedType] -= amount;
            return true;
        }
        return false;
    }

    // Backwards compatibility: gebruik deze voor "default" seeds
    public void AddSeeds(int amount)
    {
        AddSeeds("carrot", amount);
    }

    public int GetTotalSeeds()
    {
        int total = 0;
        foreach (var seed in seeds.Values)
        {
            total += seed;
        }
        return total;
    }
}