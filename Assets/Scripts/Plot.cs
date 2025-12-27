using UnityEngine;

public class Plot : MonoBehaviour
{
    public int growthStage = 0;
    public int maxGrowthStage => plantedPlant.growthSprites.Length;

    public bool isWatered = false;

    public Sprite emptySprite;

    private PlantData plantedPlant = null;

    public SpriteRenderer groundRenderer;
    public SpriteRenderer plantRenderer;

    public Sprite dryGroundSprite;
    public Sprite wetGroundSprite;



    void Awake()
    {
        
        //UpdateSprite();
    }

    void OnMouseUp()
    {
        if (SeedSelectionUI.Instance.IsMenuOpen()) return;

        if (SeedSelectionUI.ActiveSelectedPlant != null && growthStage == 0)
        {
            PlantSeed(SeedSelectionUI.ActiveSelectedPlant);
            return;
        }

        if (SeedSelectionUI.ActiveSelectedTool != null)
        {
            if (SeedSelectionUI.ActiveSelectedTool == "wateringCan")
            {
                WaterPlant();
            }
        }

        else if (plantedPlant != null && growthStage >= maxGrowthStage && SeedSelectionUI.ActiveSelectedPlant == null)
        {
            // Oogsten
            HarvestPlant();
        }
    }

    private void HarvestPlant()
    {
        if (plantedPlant != null)
        {
            CoinManager.Instance.AddCoins(plantedPlant.harvestCoins);
        }

        // Reset plot
        plantedPlant = null;
        growthStage = 0;

        UpdateSprite();
    }

    public void PlantSeed(PlantData plant)
    {
        if (plant == null)
        {
            Debug.LogError("No PlantData assigned!");
            return;
        }

        // Check of je genoeg seeds hebt
        if (SeedManager.Instance.UseSeeds(plant.seedType, 1))
        {
            plantedPlant = plant;
            growthStage = 1;

            Debug.Log($"Planted {plant.seedType}. Seeds left: {SeedManager.Instance.GetSeeds(plant.seedType)}");
            UpdateSprite();
        }
        else
        {
            Debug.Log($"You have no {plant.seedType} seeds left!");
        }
    }

    // Dag wissel
    public void AdvanceDay()
    {
        if (plantedPlant != null && growthStage > 0 && growthStage < maxGrowthStage)
        {
            growthStage++;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        // 1. Ground
        groundRenderer.sprite = isWatered ? wetGroundSprite : dryGroundSprite;

        // 2. Plant
        if (growthStage == 0 || plantedPlant == null)
        {
            plantRenderer.sprite = null;
        }
        else
        {
            plantRenderer.sprite = plantedPlant.growthSprites[growthStage - 1];
        }
    }


    public PlantData GetPlantedPlant()
    {
        return plantedPlant;
    }

    public void SetEmpty()
    {
        plantedPlant = null;
        growthStage = 0;
        UpdateSprite();
    }

    public void ForcePlantState(PlantData plant, int stage)
    {
        plantedPlant = plant;
        growthStage = stage;
        UpdateSprite();
    }

    public void WaterPlant()
    {
        
        isWatered = true;
        UpdateSprite();
    }


}
