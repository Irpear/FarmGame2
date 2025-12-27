using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Plants/New Plant")]
public class PlantData : ScriptableObject
{
    [Header("Plant Info")]
    public string seedType;          // bijvoorbeeld "carrot", "tomato"

    [Header("Sprites per groeistage")]
    public Sprite[] growthSprites;   // bijv. 4 sprites: stage 1-4

    [Header("Sprites van de aarde")]
    public Sprite groundDrySprite;
    public Sprite groundWetSprite;

    [Header("Winst/Opbrengst")]
    public int harvestCoins = 1;     // optioneel
}


