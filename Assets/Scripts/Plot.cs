using UnityEngine;

public class Plot : MonoBehaviour
{
    public int growthStage = 0;      // 0 = leeg, 1-4 = groei stages
    public int maxGrowthStage = 4;

    public Sprite emptySprite;
    public Sprite[] growthSprites;   // Array met sprites voor stage 1-4

    private SpriteRenderer sr;

    void Awake() // Was: void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void OnMouseDown()
    {
        if (growthStage == 0)
        {
            // Plant een zaadje
            growthStage = 1;
        }
        else if (growthStage >= maxGrowthStage)
        {
            // Oogst of reset
            growthStage = 0;
        }

        UpdateSprite();
    }

    // Deze functie wordt aangeroepen bij dagwisseling
    public void AdvanceDay()
    {
        if (growthStage > 0 && growthStage < maxGrowthStage)
        {
            growthStage++;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        if (growthStage == 0)
        {
            sr.sprite = emptySprite;
        }
        else
        {
            sr.sprite = growthSprites[growthStage - 1];
        }
    }
}
