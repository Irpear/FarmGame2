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

    [Header("Trashcan Sprites")]
    public Sprite defaultTrashcanSprite;
    public Sprite whiteTrashcanSprite;
    public Sprite redTrashcanSprite;

    [Header("State")]
    public bool isFull = false;
    public bool isReady = false;  // Kan alleen true zijn als isFull == true

    public bool isTrashcan = true;

    private Button button;

    public GameObject forceCompostScreen;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);

        if (PlayerPrefs.GetInt("composter_unlocked", 0) == 1)
        {
            isTrashcan = false;
        }

        UpdateVisual();
    }

    private void OnClicked()
    {
        // 1. Als leeg + speler heeft deadPlant → vul composter (rood)
        if (!isFull && SeedSelectionUI.ActiveSelectedTool == "deadPlant")
        {
            InsertDeadPlant();
            SeedSelectionUI.ActiveSelectedTool = null; // hand lege maken
            ForceCompostScreenOff();
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
        {
            if (isTrashcan) composterImage.sprite = whiteTrashcanSprite;
            else composterImage.sprite = whiteSprite;
        }
            
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
            if (isTrashcan) isFull = false;
            else isReady = true;
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
        SeedSelectionUI.ActiveSelectedPlant = null;

        SeedSelectionUI.Instance.ReturnWateringCan();

        // composter wordt leeg
        isFull = false;
        isReady = false;

        UpdateVisual();
        DayManager.Instance?.SaveComposterState(this);
    }

    private void SwitchDeadPlantAndCompost()
    {
        // Tool geven
        SeedSelectionUI.ActiveSelectedTool = "compost";
        SeedSelectionUI.ActiveSelectedPlant = null;

        // composter gaat weer aan
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
            if (isTrashcan) composterImage.sprite = defaultTrashcanSprite;
            else composterImage.sprite = defaultSprite;
        }

        // vol + niet klaar = rood
        else if (isFull && !isReady)
        {
            if (isTrashcan) composterImage.sprite = redTrashcanSprite;
            else composterImage.sprite = redSprite;
        }

        // vol + klaar = groen
        else if (isFull && isReady)
        {
            composterImage.sprite = greenSprite;
        }

    }

    public void ForceCompostScreenOn()
    {
        if (SeedSelectionUI.ActiveSelectedTool == "deadPlant")
        {
            forceCompostScreen.gameObject.SetActive(true);
        }
    }

    public void ForceCompostScreenOff()
    {
        forceCompostScreen.gameObject.SetActive(false);
    }

}
