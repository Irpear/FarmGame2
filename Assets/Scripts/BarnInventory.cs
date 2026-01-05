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
    public Button plantBookButton;
    public GameObject PlantBookPanel;

    public Image pageImage;
    public Button closeBookButton;

    public Sprite page1;
    public Sprite page2;


    void Awake()
    {
        UpdateUnlocksUI();
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

    private void UpdateUnlocksUI()
    {
        if (PlayerPrefs.GetInt("processor_unlocked", 0) == 1) foodProcessorButton.interactable = true;
        else foodProcessorButton.interactable = false;
        if (PlayerPrefs.GetInt("plantbook_unlocked", 0) == 1) plantBookButton.interactable = true;
        else plantBookButton.interactable = false;
    }

    public void OpenPlantBook()
    {
        pageImage.sprite = page1;
        PlantBookPanel.SetActive(true);
        closeBookButton.gameObject.SetActive(false);
    }
    public void OpenPlantBookPage2()
    {
        pageImage.sprite = page2;
        closeBookButton.gameObject.SetActive(true);
    }
    public void ClosePlantBook()
    {
        PlantBookPanel.SetActive(false);
    }
}
