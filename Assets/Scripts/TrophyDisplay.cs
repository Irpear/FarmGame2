using UnityEngine;
using UnityEngine.UI;

// Op elk trofee-object op de plank zetten en in de Inspector kiezen welke trofee het is.
// Niet behaald: zwart silhouet + "???". Behaald: originele afbeelding + beschrijving.
public class TrophyDisplay : MonoBehaviour
{
    public TrophyId trophy;

    [Header("Optioneel, wordt anders automatisch gezocht")]
    public Image trophyImage;   // Image op dit object
    public Text descriptionText; // (legacy) Text in een child

    void Awake()
    {
        if (trophyImage == null) trophyImage = GetComponent<Image>();
        if (descriptionText == null) descriptionText = GetComponentInChildren<Text>(true);
    }

    // Elke keer dat het trofeescherm opengaat
    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = Trophies.IsUnlocked(trophy);

        if (trophyImage != null)
            trophyImage.color = unlocked ? Color.white : Color.black;

        if (descriptionText != null)
            descriptionText.text = unlocked ? Trophies.GetDescription(trophy) : "???";
    }
}
