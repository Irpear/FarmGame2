using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chicken : MonoBehaviour
{

    private Animator animator;


    [Header("Spawn Area")]
    public float minX = -100f;
    public float maxX = 100f;
    public float minY = -100f;
    public float maxY = 100f;

    [Header("References")]
    public Image happinessBar;
    public Sprite[] happinessSprites; // 9 sprites voor happiness 0-8
    public GameObject eggPrefab;
    public Transform eggContainer; // Parent voor eieren (het panel)

    [Header("Animation Settings")]
    public float minIdleTime = 3f;
    public float maxIdleTime = 8f;

    private int chickenID;
    private RectTransform rectTransform;

    // PlayerPrefs keys
    private string UnlockedKey => $"chicken{chickenID}_unlocked";
    private string HappinessKey => $"chicken{chickenID}_happiness";
    private string PositionXKey => $"chicken{chickenID}_posX";
    private string PositionYKey => $"chicken{chickenID}_posY";

    private int happiness
    {
        get => PlayerPrefs.GetInt(HappinessKey, 3);
        set
        {
            value = Mathf.Clamp(value, 0, 8);
            PlayerPrefs.SetInt(HappinessKey, value);
            PlayerPrefs.Save();
            UpdateHappinessBar();
        }
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        animator = GetComponent<Animator>();


        // Detecteer chickenID van naam
        if (gameObject.name.Contains("Chicken1")) chickenID = 1;
        else if (gameObject.name.Contains("Chicken2")) chickenID = 2;
        else Debug.LogError("Chicken name must contain 'Chicken1' or 'Chicken2'!");

        // Check of unlocked
        bool unlocked = PlayerPrefs.GetInt(UnlockedKey, 0) == 1;

        if (!unlocked)
        {
            gameObject.SetActive(false);
            return;
        }

        
        RandomizePosition();
        UpdateHappinessBar();
        StartCoroutine(IdleAnimationLoop());
    }

    void OnEnable()
    {
        UpdateHappinessBar();
    }

    private void RandomizePosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        rectTransform.anchoredPosition = new Vector2(randomX, randomY);
        SavePosition();
    }

    private void SavePosition()
    {
        PlayerPrefs.SetFloat(PositionXKey, rectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat(PositionYKey, rectTransform.anchoredPosition.y);
        PlayerPrefs.Save();
    }

    private void LoadPosition()
    {
        float x = PlayerPrefs.GetFloat(PositionXKey);
        float y = PlayerPrefs.GetFloat(PositionYKey);
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    private void UpdateHappinessBar()
    {
        Debug.Log($"Chicken {chickenID} happiness: {happiness}");

        if (happinessBar == null)
        {
            Debug.LogError("happinessBar is null!");
            return;
        }

        if (happinessSprites.Length != 9)
        {
            Debug.LogError($"happinessSprites length is {happinessSprites.Length}, need 9!");
            return;
        }

        int index = Mathf.Clamp(happiness, 0, 8);
        Debug.Log($"Setting sprite to index {index}");
        happinessBar.sprite = happinessSprites[index];
    }

    public void OnChickenClicked()
    {
        Debug.Log($"Clicked on Chicken {chickenID}!");
        if (animator != null)
        {
            animator.SetTrigger("Click");
            Debug.Log($"animatie zou moeten werken!");
        }
    }


    private IEnumerator IdleAnimationLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(waitTime);

            // TODO: Speel idle animatie af
            // Als je later een Animator hebt: GetComponent<Animator>().SetTrigger("Idle");
        }
    }

    // Wordt aangeroepen vanuit DayManager via PlayerPrefs
    public static void ProcessChickenDay(int chickenID)
    {
        string unlockedKey = $"chicken{chickenID}_unlocked";
        string happinessKey = $"chicken{chickenID}_happiness";
        string feederKey = $"Feeder{chickenID}_full";
        string feederContentKey = $"Feeder{chickenID}_content";
        string posXKey = $"chicken{chickenID}_posX";
        string posYKey = $"chicken{chickenID}_posY";

        // Check of chicken unlocked is
        if (PlayerPrefs.GetInt(unlockedKey, 0) != 1) return;

        int currentHappiness = PlayerPrefs.GetInt(happinessKey, 3);
        bool feederFull = PlayerPrefs.GetInt(feederKey, 0) == 1;
        string feederContent = PlayerPrefs.GetString(feederContentKey, "");

        bool ateAnimalFood = false;

        // EET uit feeder
        if (feederFull)
        {
            switch (feederContent)
            {
                case "wheat":
                    currentHappiness -= 2;
                    break;
                case "animalFood":
                    ateAnimalFood = true;
                    break;
                case "corn":
                    currentHappiness += 1;
                    break;
                case "animalFood2":
                    currentHappiness += 3;
                    ateAnimalFood = true;
                    break;
            }

            // Leeg feeder
            PlayerPrefs.SetInt(feederKey, 0);
            PlayerPrefs.SetString(feederContentKey, "");
        }

        currentHappiness = Mathf.Clamp(currentHappiness, 0, 8);

        // LEG EI
        string eggType = "none";
        if (feederFull)
        {
            eggType = DetermineEggType(currentHappiness);
        }

        if (eggType == "golden")
        {
            currentHappiness -= 5;
        }

        // Sla ei op om te spawnen
        if (eggType != "none")
        {
            SaveEggToSpawn(chickenID, eggType, posXKey, posYKey);
        }

        // Verlaag happiness als geen animal food gegeten
        if (!ateAnimalFood)
        {
            currentHappiness -= 1;
        }

        currentHappiness = Mathf.Clamp(currentHappiness, 0, 8);
        PlayerPrefs.SetInt(happinessKey, currentHappiness);
        PlayerPrefs.Save();
    }

    private static string DetermineEggType(int happiness)
    {
        if (happiness <= 1)
        {
            return Random.value <= 0.5f ? "normal" : "none";
        }
        else if (happiness <= 3)
        {
            return "normal";
        }
        else if (happiness <= 5)
        {
            return Random.value <= 0.3f ? "large" : "normal";
        }
        else if (happiness <= 7)
        {
            return "large";
        }
        else // happiness == 8
        {
            return Random.value <= 0.3f ? "golden" : "large";
        }
    }

    private static void SaveEggToSpawn(int chickenID, string eggType, string posXKey, string posYKey)
    {
        // Sla ei info op om later te spawnen
        int eggCount = PlayerPrefs.GetInt($"eggs_to_spawn_count", 0);

        PlayerPrefs.SetInt($"egg_{eggCount}_chickenID", chickenID);
        PlayerPrefs.SetString($"egg_{eggCount}_type", eggType);

        // Gebruik chicken positie voor ei spawn
        float chickenX = PlayerPrefs.GetFloat(posXKey, 0);
        float chickenY = PlayerPrefs.GetFloat(posYKey, 0);

        PlayerPrefs.SetFloat($"egg_{eggCount}_x", chickenX);
        PlayerPrefs.SetFloat($"egg_{eggCount}_y", chickenY);

        eggCount++;
        PlayerPrefs.SetInt($"eggs_to_spawn_count", eggCount);
        PlayerPrefs.Save();
    }

    // Roep aan wanneer chicken gekocht wordt
    public static void UnlockChicken(int chickenID)
    {
        string unlockedKey = $"chicken{chickenID}_unlocked";
        string happinessKey = $"chicken{chickenID}_happiness";

        PlayerPrefs.SetInt(unlockedKey, 1);
        PlayerPrefs.SetInt(happinessKey, 3); // Start happiness
        PlayerPrefs.Save();
    }
}