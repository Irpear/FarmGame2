using TMPro;
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

    public bool full = false;
    public bool done = false;
    public bool wheat = true;


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
