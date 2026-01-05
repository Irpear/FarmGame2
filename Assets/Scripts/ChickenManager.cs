using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenManager : MonoBehaviour
{
    public Button feeder2;

    public Image feederImage;
    public Image feederImage2;
    public Sprite feederFull;
    public Sprite feederEmpty;

    public GameObject foodSelectionPanel;
    public GameObject closeButton;

    public Button wheatButton;
    public Button animalFoodButton;
    public Button cornButton;
    public Button animalFood2Button;

    public TextMeshProUGUI wheatText;
    public TextMeshProUGUI animalFoodText;
    public TextMeshProUGUI cornText;
    public TextMeshProUGUI animalFood2Text;

    private int activeFeeder = 1; // Track welke feeder actief is

    void Awake()
    {
        UpdateUI();
    }

    // Helper functies voor PlayerPrefs
    private bool GetFeederFull(int feederID)
    {
        return PlayerPrefs.GetInt($"Feeder{feederID}_full", 0) == 1;
    }

    private void SetFeederFull(int feederID, bool value)
    {
        PlayerPrefs.SetInt($"Feeder{feederID}_full", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private string GetFeederContent(int feederID)
    {
        return PlayerPrefs.GetString($"Feeder{feederID}_content", "");
    }

    private void SetFeederContent(int feederID, string value)
    {
        PlayerPrefs.SetString($"Feeder{feederID}_content", value);
        PlayerPrefs.Save();
    }

    // Click functies
    public void ClickFeeder() => OpenFeederPanel(1);
    public void ClickFeeder2() => OpenFeederPanel(2);

    private void OpenFeederPanel(int feederID)
    {
        if (GetFeederFull(feederID)) return;

        activeFeeder = feederID;
        foodSelectionPanel.SetActive(true);
        closeButton.gameObject.SetActive(true);
        UpdateFoodCounts();
    }

    // Select functies (nu gebruiken ze activeFeeder)
    public void SelectWheat()
    {
        CoinManager.Instance.AddWheat(-1);
        SetFeederFull(activeFeeder, true);
        SetFeederContent(activeFeeder, "wheat");
        ClosePanel();
    }

    public void SelectAnimalFood()
    {
        CoinManager.Instance.AddAnimalFood(-1);
        SetFeederFull(activeFeeder, true);
        SetFeederContent(activeFeeder, "animalFood");
        ClosePanel();
    }

    public void SelectCorn()
    {
        CoinManager.Instance.AddCorn(-1);
        SetFeederFull(activeFeeder, true);
        SetFeederContent(activeFeeder, "corn");
        ClosePanel();
    }

    public void SelectAnimalFood2()
    {
        CoinManager.Instance.AddAnimalFood2(-1);
        SetFeederFull(activeFeeder, true);
        SetFeederContent(activeFeeder, "animalFood2");
        ClosePanel();
    }

    private void ClosePanel()
    {
        UpdateUI();
        foodSelectionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void CloseFoodSelectionPanel()
    {
        foodSelectionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void UpdateFoodCounts()
    {
        int animalFoodCount = CoinManager.Instance.animalFood;
        animalFoodText.text = animalFoodCount > 0 ? $"Animal food ({animalFoodCount})" : "";
        animalFoodButton.interactable = animalFoodCount > 0;

        int wheatCount = CoinManager.Instance.wheatResource;
        wheatText.text = wheatCount > 0 ? $"Wheat ({wheatCount})" : "";
        wheatButton.interactable = wheatCount > 0;

        int animalFood2Count = CoinManager.Instance.animalFood2;
        animalFood2Text.text = animalFood2Count > 0 ? $"Animal food 2 ({animalFood2Count})" : "";
        animalFood2Button.interactable = animalFood2Count > 0;

        int cornCount = CoinManager.Instance.cornResource;
        cornText.text = cornCount > 0 ? $"Corn ({cornCount})" : "";
        cornButton.interactable = cornCount > 0;
    }

    private void UpdateUI()
    {
        if (PlayerPrefs.GetInt("feeder2_unlocked", 0) == 1) { feeder2.interactable = true; }
        else { feeder2.interactable = false; }


            feederImage.sprite = GetFeederFull(1) ? feederFull : feederEmpty;
        feederImage2.sprite = GetFeederFull(2) ? feederFull : feederEmpty;
    }
}