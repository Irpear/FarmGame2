using System;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public int growthStage = 0;      // 0 = leeg, 1-4 = groei stages
    public int maxGrowthStage = 4;

    public Sprite emptySprite;
    public Sprite[] growthSprites;   // Array met sprites voor stage 1-4

    private SpriteRenderer sr;

    private string plantedSeedType = "";

    void Awake() // Was: void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void OnMouseUp()
    {
        if (growthStage == 0)
        {
            // Toon seed selectie menu
            SeedSelectionUI.Instance.ShowSelectionMenu(this);
        }
        else if (growthStage >= maxGrowthStage)
        {
            // Oogst of reset
            growthStage = 0;
            plantedSeedType = "";
            CoinManager.Instance.AddCoins(1);
            UpdateSprite();
        }
    }

    public void PlantSeed(string seedType)
    {
        if (SeedManager.Instance.UseSeeds(seedType, 1))
        {
            growthStage = 1;
            plantedSeedType = seedType;
            Debug.Log($"Planted {seedType}. You have {SeedManager.Instance.GetSeeds(seedType)} {seedType} seeds left");
            UpdateSprite();
        }
        else
        {
            Debug.Log($"You have no {seedType} seeds left!");
        }
    }

    // Deze functie wordt aangeroepen bij dagwisseling
    public void AdvanceDay()
    {
        if (growthStage > 0 && growthStage < maxGrowthStage)
        {
            growthStage++;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        if (growthStage == 0)
        {
            sr.sprite = emptySprite;
        }
        else
        {
            sr.sprite = growthSprites[growthStage - 1];
        }
    }

    public string GetPlantedSeedType()
    {
        return plantedSeedType;
    }
}
