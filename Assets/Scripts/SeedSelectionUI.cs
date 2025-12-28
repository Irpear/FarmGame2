using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedSelectionUI : MonoBehaviour
{
    public static SeedSelectionUI Instance;

    public PlantData[] allPlants; // Vul dit in inspector met al je PlantData assets

    public GameObject selectionPanel;  // Panel dat je aan/uit zet
    public Button closeButton;
    public Button carrotButton;
    public Button tomatoButton;
    public Button wheatButton;
    public Button cornButton;
    public Button grapeButton;
    public Button potatoButton;

    public TextMeshProUGUI carrotText;
    public TextMeshProUGUI tomatoText;
    public TextMeshProUGUI wheatText;
    public TextMeshProUGUI cornText;
    public TextMeshProUGUI grapeText;
    public TextMeshProUGUI potatoText;

    public PlantData carrotData;
    public PlantData tomatoData;
    public PlantData wheatData;
    public PlantData cornData;
    public PlantData grapeData;
    public PlantData potatoData;

    public static PlantData ActiveSelectedPlant = null;

    public static string ActiveSelectedTool = null;

    public Image wateringCanImage;
    public Sprite WateringCan;
    public Sprite WateringCanGlow;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Zorg dat panel uit staat bij start
        closeButton.gameObject.SetActive(false);
        selectionPanel.SetActive(false);

        // Koppel buttons
        closeButton.onClick.AddListener(HideSelectionMenu);
        carrotButton.onClick.AddListener(() => SelectSeed(carrotData));
        tomatoButton.onClick.AddListener(() => SelectSeed(tomatoData));
        wheatButton.onClick.AddListener(() => SelectSeed(wheatData));
        cornButton.onClick.AddListener(() => SelectSeed(cornData));
        grapeButton.onClick.AddListener(() => SelectSeed(grapeData));
        potatoButton.onClick.AddListener(() => SelectSeed(potatoData));


    }

    public void ShowSelectionMenu()
    {
        UpdateSeedCounts();
        closeButton.gameObject.SetActive(true);
        selectionPanel.SetActive(true);
    }


    public void HideSelectionMenu()
    {
        closeButton.gameObject.SetActive(false);
        selectionPanel.SetActive(false);
    }

    private void SelectSeed(PlantData plant)
    {
        ActiveSelectedPlant = plant;   // Onthoud de gekozen plant
        ActiveSelectedTool = null;

        ReturnWateringCan();

        closeButton.gameObject.SetActive(false);
        selectionPanel.SetActive(false);
        
    }


    private void UpdateSeedCounts()
    {
        carrotText.text = $"Carrot ({SeedManager.Instance.GetSeeds("carrot")})";
        tomatoText.text = $"Tomato ({SeedManager.Instance.GetSeeds("tomato")})";
        wheatText.text = $"Wheat ({SeedManager.Instance.GetSeeds("wheat")})";
        cornText.text = $"Corn ({SeedManager.Instance.GetSeeds("corn")})";
        grapeText.text = $"Grape ({SeedManager.Instance.GetSeeds("grape")})";
        potatoText.text = $"Potato ({SeedManager.Instance.GetSeeds("potato")})";
    }

    public bool IsMenuOpen()
    {
        return selectionPanel.activeSelf;
    }

    public void ClearActiveSeed()
    {
        ActiveSelectedPlant = null;
        closeButton.gameObject.SetActive(false);
        selectionPanel.SetActive(false);
    }

    public PlantData GetPlantDataByType(string type)
    {
        foreach (var plant in allPlants)
        {
            if (plant != null && plant.seedType == type)
                return plant;
        }
        return null;
    }

    public void SelectWateringCan()
    {
        if (ActiveSelectedTool == "wateringCan")
        {
            ActiveSelectedTool = null;
            ReturnWateringCan();
        }
        else
        {
            ActiveSelectedTool = "wateringCan";
            TakeWateringCan();
        }
        ActiveSelectedPlant = null;
    }

    public void TakeWateringCan()
    {
        if (wateringCanImage != null)
        {
            wateringCanImage.sprite = WateringCanGlow;
        }
    }

    public void ReturnWateringCan()
    {
        if (wateringCanImage != null)
        {
            wateringCanImage.sprite = WateringCan;
        }
    }
}
