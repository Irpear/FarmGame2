using UnityEngine;
using UnityEngine.UI;

public class Composter : MonoBehaviour
{
    [Header("UI")]
    public Image composterImage;

    [Header("Sprites")]
    public Sprite defaultSprite; // standaard zonder interactie
    public Sprite whiteSprite;   // highlight wanneer deadPlant geselecteerd
    public Sprite redSprite;     // vol + niet klaar
    public Sprite greenSprite;   // vol + klaar

    [Header("State")]
    public bool isFull = false;
    public bool isReady = false;  // Kan alleen true zijn als isFull == true

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);

        //UpdateVisual();
    }

    private void OnClicked()
    {
        // 1. Als leeg + speler heeft deadPlant → vul composter (rood)
        if (!isFull && SeedSelectionUI.ActiveSelectedTool == "deadPlant")
        {
            InsertDeadPlant();
            SeedSelectionUI.ActiveSelectedTool = null; // hand lege maken
            return;
        }

        // 2. Als VOL & KLAAR → speler krijgt compost
        if (isFull && isReady)
        {
            GiveCompostToPlayer();
            return;
        }

        // 3. Als LEEG + speler probeert compost terug te leggen
        if (!isFull && SeedSelectionUI.ActiveSelectedTool == "compost")
        {
            ReturnCompost();
            return;
        }
    }


    public void ShowDeadPlantHighlight()
    {
        if (!isFull)
            composterImage.sprite = whiteSprite;
    }

    public void ClearHighlight()
    {
        UpdateVisual();
    }

    public void ProcessNewDay()
    {
        // Alleen transformeren als hij vol & niet klaar is → wordt nu klaar
        if (isFull && !isReady)
        {
            isReady = true;
            UpdateVisual();
        }
        DayManager.Instance?.SaveComposterState(this);
    }

    private void InsertDeadPlant()
    {
        isFull = true;
        isReady = false;
        UpdateVisual();
        DayManager.Instance?.SaveComposterState(this);
    }

    private void GiveCompostToPlayer()
    {
        // Tool geven
        SeedSelectionUI.ActiveSelectedTool = "compost";

        // composter wordt leeg
        isFull = false;
        isReady = false;

        UpdateVisual();
        DayManager.Instance?.SaveComposterState(this);
    }

    private void ReturnCompost()
    {
        SeedSelectionUI.ActiveSelectedTool = null;

        isFull = true;
        isReady = true;

        UpdateVisual();
        DayManager.Instance?.SaveComposterState(this);
    }


    public void UpdateVisual()
    {
        if (!isFull)
        {
            composterImage.sprite = defaultSprite;
        }

        // vol + niet klaar = rood
        else if (isFull && !isReady)
        {
            composterImage.sprite = redSprite;
        }

        // vol + klaar = groen
        else if (isFull && isReady)
        {
            composterImage.sprite = greenSprite;
        }

    }


}
