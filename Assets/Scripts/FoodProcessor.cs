using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FoodProcessor : MonoBehaviour
{

    public Image processorImage;

    public Sprite processorDefault;
    public Sprite processorBusy;
    public Sprite processorDone;


    public GameObject processorPanel;
    public GameObject closeButton;

    public Button wheatResourceButton;
    public Button cornResourceButton;

    public TextMeshProUGUI wheatResourceText;
    public TextMeshProUGUI cornResourceText;

    private const string PREF_FULL = "Processor_full";
    private const string PREF_DONE = "Processor_done";
    private const string PREF_WHEAT = "Processor_wheat";

    public bool full
    {
        get => PlayerPrefs.GetInt(PREF_FULL, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_FULL, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public bool done
    {
        get => PlayerPrefs.GetInt(PREF_DONE, 0) == 1;
        set { PlayerPrefs.SetInt(PREF_DONE, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public bool wheat
    {
        get => PlayerPrefs.GetInt(PREF_WHEAT, 1) == 1; // Default = true (wheat)
        set { PlayerPrefs.SetInt(PREF_WHEAT, value ? 1 : 0); PlayerPrefs.Save(); }
    }


    void Awake()
    {
        UpdateUI();
    }

    public void ClickProcessor()
    {
        if (full) return;
        else if (done)
        {
            if (wheat) { CoinManager.Instance.AddAnimalFood(1); }
            if (!wheat) { CoinManager.Instance.AddAnimalFood2(1); }
            done = false;
        }

        else
        {
            processorPanel.SetActive(true);
            closeButton.gameObject.SetActive(true);
            UpdateFoodCounts();
        }
        UpdateUI();
    }

    public void CloseProcessorPanel()
    {
        processorPanel.SetActive(false);
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
    }

    public void SelectWheat()
    {
        CoinManager.Instance.AddWheat(-1);
        full = true;
        wheat = true;
        UpdateUI();
        processorPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void SelectCorn()
    {
        CoinManager.Instance.AddCorn(-1);
        full = true;
        wheat = false; // dus het is mais
        UpdateUI();
        processorPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (full) processorImage.sprite = processorBusy;
        else if (done) processorImage.sprite = processorDone;
        else processorImage.sprite = processorDefault;
    }
}
