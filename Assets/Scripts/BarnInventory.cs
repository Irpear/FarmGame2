using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarnInventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject closeButton;

    public Button wheatResourceButton; //zonder klikfunctie gewoon makkelijk voor spritewissel
    public Button cornResourceButton;
    public Button animalFoodButton;
    public Button animalFood2Button;

    public TextMeshProUGUI wheatResourceText;
    public TextMeshProUGUI cornResourceText;
    public TextMeshProUGUI animalFoodText;
    public TextMeshProUGUI animalFood2Text;

    public Button foodProcessorButton;


    void Awake()
    {
        UpdateProcessorUI();
    }


    public void ClickInventory()
    {
        inventoryPanel.SetActive(true);
        closeButton.gameObject.SetActive(true);
        UpdateFoodCounts();
    }

    public void CloseInventoryPanel()
    {
        inventoryPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void UpdateFoodCounts()
    {
        // Wheat
        int wheatCount = CoinManager.Instance.wheatResource;
        wheatResourceText.text = wheatCount > 0 ? $"Wheat ({wheatCount})" : "";
        wheatResourceButton.interactable = wheatCount > 0;
        // Corn
        int cornCount = CoinManager.Instance.cornResource;
        cornResourceText.text = cornCount > 0 ? $"Corn ({cornCount})" : "";
        cornResourceButton.interactable = cornCount > 0;
        // Animal Food
        int animalFoodCount = CoinManager.Instance.animalFood;
        animalFoodText.text = animalFoodCount > 0 ? $"Animal food ({animalFoodCount})" : "";
        animalFoodButton.interactable = animalFoodCount > 0;
        // Animal Food 2
        int animalFood2Count = CoinManager.Instance.animalFood2;
        animalFood2Text.text = animalFood2Count > 0 ? $"Animal food 2 ({animalFood2Count})" : "";
        animalFood2Button.interactable = animalFood2Count > 0;

    }

    private void UpdateProcessorUI()
    {
        if (PlayerPrefs.GetInt("processor_unlocked", 0) == 1) foodProcessorButton.interactable = true;
        else foodProcessorButton.interactable = false;
    }
}
