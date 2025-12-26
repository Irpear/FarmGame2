using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedSelectionUI : MonoBehaviour
{
    public static SeedSelectionUI Instance;

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

    private Plot currentPlot;  // Welke plot is geselecteerd

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
        carrotButton.onClick.AddListener(() => SelectSeed("carrot"));
        tomatoButton.onClick.AddListener(() => SelectSeed("tomato"));
        wheatButton.onClick.AddListener(() => SelectSeed("wheat"));
        cornButton.onClick.AddListener(() => SelectSeed("corn"));
        grapeButton.onClick.AddListener(() => SelectSeed("grape"));
        potatoButton.onClick.AddListener(() => SelectSeed("potato"));
        
    }

    public void ShowSelectionMenu(Plot plot)
    {
        currentPlot = plot;
        UpdateSeedCounts();
        closeButton.gameObject.SetActive(true);
        selectionPanel.SetActive(true);
    }

    public void HideSelectionMenu()
    {
        closeButton.gameObject.SetActive(false);
        selectionPanel.SetActive(false);
        currentPlot = null;
    }

    private void SelectSeed(string seedType)
    {
        if (currentPlot != null)
        {
            currentPlot.PlantSeed(seedType);
            HideSelectionMenu();
        }
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
}
