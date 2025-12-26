using UnityEngine;

public class Plot : MonoBehaviour
{
    public int growthStage = 0;
    public int maxGrowthStage => plantedPlant.growthSprites.Length;


    public Sprite emptySprite;

    private SpriteRenderer sr;
    private PlantData plantedPlant = null;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void OnMouseUp()
    {
        if (SeedSelectionUI.Instance.IsMenuOpen()) return;

        if (growthStage == 0)
        {
            // Toon selectie menu
            SeedSelectionUI.Instance.ShowSelectionMenu(this);
        }
        else if (growthStage >= maxGrowthStage)
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
        if (growthStage > 0 && growthStage < maxGrowthStage)
        {
            growthStage++;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        if (growthStage == 0 || plantedPlant == null)
        {
            sr.sprite = emptySprite;
            return;
        }

        // Veiligheidscheck
        if (plantedPlant.growthSprites == null || plantedPlant.growthSprites.Length < growthStage)
        {
            Debug.LogError($"PlantData for {plantedPlant.seedType} has not enough sprites!");
            return;
        }

        sr.sprite = plantedPlant.growthSprites[growthStage - 1];
    }

    public PlantData GetPlantedPlant()
    {
        return plantedPlant;
    }
}
