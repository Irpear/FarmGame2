using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenManager : MonoBehaviour
{
    public Image feederImage;

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

    private const string PREF_KEY = "Feeder1_full";

    private bool full
    {
        get => PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_KEY, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    void Awake()
    {
        UpdateUI();
    }

    public void ClickFeeder()
    {
        if (full) return;
        else
        {
            foodSelectionPanel.SetActive(true);
            closeButton.gameObject.SetActive(true);
            UpdateFoodCounts();
        }
    }

    public void SelectWheat()
    {
        CoinManager.Instance.AddWheat(-1);
        full = true;

        UpdateUI();
        foodSelectionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void SelectAnimalFood()
    {
        CoinManager.Instance.AddAnimalFood(-1);
        full = true;

        UpdateUI();
        foodSelectionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void SelectCorn()
    {
        CoinManager.Instance.AddCorn(-1);
        full = true;

        UpdateUI();
        foodSelectionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void SelectAnimalFood2()
    {
        CoinManager.Instance.AddAnimalFood2(-1);
        full = true;

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
        // Animal Food
        int animalFoodCount = CoinManager.Instance.animalFood;
        animalFoodText.text = animalFoodCount > 0 ? $"Animal food ({animalFoodCount})" : "";
        animalFoodButton.interactable = animalFoodCount > 0;

        // Wheat
        int wheatCount = CoinManager.Instance.wheatResource;
        wheatText.text = wheatCount > 0 ? $"Wheat ({wheatCount})" : "";
        wheatButton.interactable = wheatCount > 0;

        // Animal Food 2
        int animalFood2Count = CoinManager.Instance.animalFood2;
        animalFood2Text.text = animalFood2Count > 0 ? $"Animal food 2 ({animalFood2Count})" : "";
        animalFood2Button.interactable = animalFood2Count > 0;

        // Corn
        int cornCount = CoinManager.Instance.cornResource;
        cornText.text = cornCount > 0 ? $"Corn ({cornCount})" : "";
        cornButton.interactable = cornCount > 0;
    }

    private void UpdateUI()
    {
        if (full) feederImage.sprite = feederFull;
        else feederImage.sprite = feederEmpty;
    }
}
