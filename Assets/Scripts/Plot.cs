using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Plot : MonoBehaviour
{
    public int growthStage = 0;
    public int maxGrowthStage => plantedPlant.growthSprites.Length;

    public bool isWatered = false;

    public bool dead = false;

    public bool composted = false;

    public Sprite emptySprite;

    public PlantData plantedPlant = null;

    public SpriteRenderer groundRenderer;
    public SpriteRenderer plantRenderer;

    public Sprite dryGroundSprite;
    public Sprite wetGroundSprite;

    public Sprite deadPlant;

    public HarvestPopup popupPrefab;

    public SpriteRenderer compostEffect;

    public SpriteRenderer shinyEffect;
    public float shinyChancePercent = 1;
    public bool isShiny = false;

    public int chosenVariant = 0;

    // ---- GRAPE SYSTEEM ----
    public bool isGrape = false;
    public int grapeMaxHarvests = 0;   // bepaald bij planten
    public int grapeHarvestsDone = 0;  // teller


    void Awake()
    {
        
        //UpdateSprite();
    }

    void OnMouseUp()
    {
        if (SeedSelectionUI.Instance.IsMenuOpen()) return;

        if (SeedSelectionUI.ActiveSelectedPlant != null && dead == true)
        {
            Composter comp = FindAnyObjectByType<Composter>();
            if (comp != null && comp.isFull)
            {
                NotificationManager.Instance.ShowNotification("Can't pick up more dead plants, the composter is already full!");
                return; // Stop hier, pak plant NIET op
            }

            SeedSelectionUI.ActiveSelectedPlant = null; 
            SeedSelectionUI.ActiveSelectedTool = "deadPlant";
            HarvestPlant();
            return;
        }

        if (SeedSelectionUI.ActiveSelectedPlant != null && growthStage == 0)
        {
            PlantSeed(SeedSelectionUI.ActiveSelectedPlant);
            return;
        }

        if (SeedSelectionUI.ActiveSelectedTool != null) // oogst dus ook niet met een deadplant of andere tools in je hand
        {
            if (SeedSelectionUI.ActiveSelectedTool == "wateringCan")
            {
                WaterPlant();
            }

            if (SeedSelectionUI.ActiveSelectedTool == "compost")
            {
                if (growthStage == 0)
                {
                    composted = true;
                    compostEffect.enabled = composted;
                    SeedSelectionUI.ActiveSelectedTool = null;
                }
                else
                {
                    NotificationManager.Instance.ShowNotification("Place compost on an empty plot.");
                }
                
            }
        }

        else if (plantedPlant != null && SeedSelectionUI.ActiveSelectedPlant == null &&
        (dead || growthStage >= maxGrowthStage))
        {
            if (dead)
            {
                Composter comp = FindAnyObjectByType<Composter>();
                if (comp != null && comp.isFull)
                {
                    NotificationManager.Instance.ShowNotification("Can't pick up more dead plants, the composter is already full!");
                    return;
                }
            }
            // Oogsten
            HarvestPlant();
        }
    }

    private void HarvestPlant()
    {

        if (isGrape)
        {
            HarvestGrape();
            return;
        }


        if (plantedPlant != null)
        {

            if (dead)
            {
                // Dead plant → in hand
                SeedSelectionUI.ActiveSelectedTool = "deadPlant";
                Composter comp = FindAnyObjectByType<Composter>();
                if (comp != null)
                {
                    comp.ForceCompostScreenOn();
                }
                FindAnyObjectByType<Composter>()?.ShowDeadPlantHighlight();
            }

            else
            {
                int coins = plantedPlant.harvestCoins;

                if (composted == true)
                {
                    if (plantedPlant.seedType == "potato")
                    {
                        coins = coins + coins;
                    }
                    else
                    {
                        coins = coins + (coins / 2);
                    }   
                }

                if (isShiny == true)
                {
                    coins = coins * 5;
                }

                // --- POPUP AANMAKEN ---
                if (popupPrefab != null)
                {
                    // Zoek canvas
                    Canvas canvas = FindFirstObjectByType<Canvas>();

                    if (canvas != null)
                    {
                        var popup = Instantiate(popupPrefab, canvas.transform);

                        // Popup positie = boven de plot
                        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

                        popup.Show(coins, screenPos);
                    }
                }

                // Voeg coins toe
                CoinManager.Instance.AddCoins(coins);
            }
        }

        //// Reset plot
        //plantedPlant = null;
        //growthStage = 0;
        //dead = false;
        isWatered = false;
        //composted = false;
        //compostEffect.enabled = composted;

        ResetPlant();

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
            chosenVariant = Random.Range(0, 4);

            ShinyRoll();

            if (SeedManager.Instance.GetSeeds(plant.seedType) == 0)
            {
                SeedSelectionUI.ActiveSelectedPlant = null;

                NotificationManager.Instance.ShowNotification($"You have no {plant.seedType} seeds left!");
            }
            
            Debug.Log($"Planted {plant.seedType}. Seeds left: {SeedManager.Instance.GetSeeds(plant.seedType)}");

            if (plant.seedType == "grape")
            {
                isGrape = true;
                grapeHarvestsDone = 0;

                // 25% kans op 1–4 harvests
                grapeMaxHarvests = Random.Range(1, 5);

                Debug.Log($"Grape planted: {grapeMaxHarvests} harvests possible.");
            }
            else
            {
                isGrape = false;
            }

            UpdateSprite();
        }
        else
        {
            SeedSelectionUI.ActiveSelectedPlant = null;

            NotificationManager.Instance.ShowNotification($"You have no {plant.seedType} seeds left!");
        }
    }

    // Dag wissel
    public void AdvanceDay()
    {
        if (plantedPlant != null && growthStage > 0 && !dead)
        {
            // Als niet water → dood
            if (!isWatered && growthStage < maxGrowthStage)
            {
                if (growthStage == 1)
                {
                    ResetPlant();
                }
                else
                {
                    if (plantedPlant.seedType != "carrot")
                    {
                        dead = true;
                    }
                }
                    
            }

            if (plantedPlant != null && growthStage == maxGrowthStage)
            {
                ResetPlant();
                DayManager.Instance.anyPlantsEaten = true;
            }

            // Als wel water → groeien
            else if (isWatered && growthStage < maxGrowthStage)
            {
                growthStage++;
            }

            else if (plantedPlant != null && plantedPlant.seedType == "carrot" && !isWatered && growthStage < maxGrowthStage)
            {
                growthStage++;
            }

        }

        isWatered = false;

        UpdateSprite();
    }

    public void UpdateSprite()
    {

        compostEffect.enabled = composted;
        shinyEffect.enabled = isShiny;

        // 1. Ground
        groundRenderer.sprite = isWatered ? wetGroundSprite : dryGroundSprite;

        // 2. Plant ja of nee
        if (growthStage == 0 || plantedPlant == null)
        {
            plantRenderer.sprite = null;
            return;
        }

        // 3. Plant is dood? Toon dood
        if (dead)
        {
            plantRenderer.sprite = deadPlant;
            return;
        }

        // 4. Normale groei
        plantRenderer.sprite = plantedPlant.growthSprites[growthStage - 1];

        if (growthStage == 1)
        {
            plantRenderer.sprite = plantedPlant.seedVariants[chosenVariant];
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

    public void ResetPlant()
    {
        plantedPlant = null;
        growthStage = 0;
        dead = false;
        composted = false;
        compostEffect.enabled = composted;
        isShiny = false;
        shinyEffect.enabled = isShiny;
        isGrape = false;
        grapeMaxHarvests = 0;
        grapeHarvestsDone = 0;
    }

    public void ShinyRoll()
    {
        if (plantedPlant.seedType == "tomato")
        {
            if (Random.value <= (shinyChancePercent / 50f)) { isShiny = true; }
        }
        else if (Random.value <= (shinyChancePercent / 100f))
        {
            isShiny = true;
        }
    }

    private void HarvestGrape()
    {
        grapeHarvestsDone++;

        int coins = 0;

        // Avond-per-dag winst
        switch (grapeHarvestsDone)
        {
            case 1: coins = 3; break;
            case 2: coins = 3; break;
            case 3: coins = 4; break;
            case 4: coins = 5; break;
        }

        // Popup
        if (popupPrefab != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                var popup = Instantiate(popupPrefab, canvas.transform);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                popup.Show(coins, screenPos);
            }
        }

        // Coins toevoegen
        CoinManager.Instance.AddCoins(coins);

        // Check einde
        if (grapeHarvestsDone >= grapeMaxHarvests)
        {
            // Druif is klaar → reset
            ResetPlant();
            UpdateSprite();
            return;
        }

        // Nog harvests te gaan → growthStage -1 zodat hij morgen weer harvestbaar is
        if (growthStage > 1)
        {
            growthStage--;
        }

        // Maar hij moet nog steeds een druif zijn
        dead = false;
        isWatered = false;

        UpdateSprite();
    }


}
