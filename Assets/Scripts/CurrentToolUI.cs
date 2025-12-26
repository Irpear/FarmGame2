using UnityEngine;
using UnityEngine.UI;

public class CurrentToolUI : MonoBehaviour
{
    public Image currentToolImage;

    [System.Serializable]
    public struct ToolSprite
    {
        public string toolName; // bijvoorbeeld "hand", "carrot", "tomato"
        public Sprite sprite;
    }

    public ToolSprite[] toolSprites;

    private void Update()
    {
        UpdateToolSprite();
    }

    void UpdateToolSprite()
    {
        Sprite newSprite = null;

        // 1. Default: hand
        string currentToolName = "hand";

        // 2. Check of er een actieve plant is
        if (SeedSelectionUI.ActiveSelectedPlant != null)
        {
            currentToolName = SeedSelectionUI.ActiveSelectedPlant.seedType;
        }

        // 3. Zoek sprite met die naam
        foreach (var ts in toolSprites)
        {
            if (ts.toolName == currentToolName)
            {
                newSprite = ts.sprite;
                break;
            }
        }

        // 4. Zet de image, fallback op hand als sprite niet gevonden
        if (newSprite == null)
        {
            // Zoek handje
            foreach (var ts in toolSprites)
            {
                if (ts.toolName == "hand")
                {
                    newSprite = ts.sprite;
                    break;
                }
            }
        }

        currentToolImage.sprite = newSprite;
    }
}
