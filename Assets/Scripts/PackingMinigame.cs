using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PackingMinigame : MonoBehaviour
{
    [Header("Display Settings")]
    public Image displayImage; // Groot beeld waar seeds getoond worden
    public float displayTime = 1f; // Hoe lang elke seed zichtbaar is
    public int orderSize = 5; // Hoeveel seeds in de order

    [Header("Seed Sprites")]
    public Sprite carrotSprite;
    public Sprite tomatoSprite;
    public Sprite wheatSprite;
    public Sprite cornSprite;
    public Sprite grapeSprite;
    public Sprite potatoSprite;

    [Header("Clickable Buttons (alle 6)")]
    public Button carrotButton;
    public Button tomatoButton;
    public Button wheatButton;
    public Button cornButton;
    public Button grapeButton;
    public Button potatoButton;

    [Header("Basket (optioneel)")]
    public Transform basketContainer; // Container voor mini sprites
    public GameObject miniSeedPrefab; // Prefab met Image component voor mini seeds

    [Header("The rest")]
    public GameObject wrongYell;
    public GameObject correctYell;
    public bool Correct;
    private bool done = false;

    private List<string> correctOrder = new List<string>();
    private List<string> playerOrder = new List<string>();
    private bool isShowingOrder = true;

    void Start()
    {
        int streak = PlayerPrefs.GetInt("PackingStreak", 0);

        orderSize = 5 + (streak * 1);

        SetupButtons();
        StartCoroutine(ShowOrder());
    }

    private void SetupButtons()
    {
        // Disable buttons tijdens show
        SetButtonsInteractable(false);

        // Koppel click events
        carrotButton.onClick.AddListener(() => OnSeedClicked("carrot"));
        tomatoButton.onClick.AddListener(() => OnSeedClicked("tomato"));
        wheatButton.onClick.AddListener(() => OnSeedClicked("wheat"));
        cornButton.onClick.AddListener(() => OnSeedClicked("corn"));
        grapeButton.onClick.AddListener(() => OnSeedClicked("grape"));
        potatoButton.onClick.AddListener(() => OnSeedClicked("potato"));
    }

    private IEnumerator ShowOrder()
    {
        isShowingOrder = true;
        correctOrder.Clear();

        // Genereer random order
        string[] seedTypes = { "carrot", "tomato", "wheat", "corn", "grape", "potato" };

        for (int i = 0; i < orderSize; i++)
        {
            string randomSeed = seedTypes[Random.Range(0, seedTypes.Length)];
            correctOrder.Add(randomSeed);
            displayImage.sprite = null;
            yield return new WaitForSeconds(0.1f);

            // Toon seed
            displayImage.sprite = GetSpriteForSeed(randomSeed);
            displayImage.enabled = true;

            yield return new WaitForSeconds(displayTime);
        }

        // Hide display
        displayImage.enabled = false;

        // Enable buttons voor input
        isShowingOrder = false;
        SetButtonsInteractable(true);

        Debug.Log($"Correct order: {string.Join(", ", correctOrder)}");
    }

    private Sprite GetSpriteForSeed(string seedType)
    {
        return seedType switch
        {
            "carrot" => carrotSprite,
            "tomato" => tomatoSprite,
            "wheat" => wheatSprite,
            "corn" => cornSprite,
            "grape" => grapeSprite,
            "potato" => potatoSprite,
            _ => null
        };
    }

    private void OnSeedClicked(string seedType)
    {
        if (isShowingOrder || done == true) return;

        playerOrder.Add(seedType);

        // Spawn mini seed in basket (optioneel)
        if (basketContainer != null && miniSeedPrefab != null)
        {
            GameObject mini = Instantiate(miniSeedPrefab, basketContainer);
            Image miniImage = mini.GetComponent<Image>();
            if (miniImage != null)
            {
                miniImage.sprite = GetSpriteForSeed(seedType);
            }

            RectTransform rt = mini.GetComponent<RectTransform>();
            if (rt != null)
            {
                float randomX = Random.Range(-100f, 100f);
                float randomY = Random.Range(0f, 50f);
                rt.anchoredPosition = new Vector2(randomX, randomY);
            }
        }

        Debug.Log($"Clicked: {seedType} ({playerOrder.Count}/{orderSize})");

        // Check of order compleet is
        if (playerOrder.Count >= orderSize)
        {
            CheckOrder();
        }
    }

    private void CheckOrder()
    {
        bool correct = true;
        done = true;

        for (int i = 0; i < orderSize; i++)
        {
            if (playerOrder[i] != correctOrder[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            OnCorrectOrder();
        }
        else
        {
            OnWrongOrder();
        }
        StartCoroutine(CloseMinigame());
    }

    private void SetButtonsInteractable(bool interactable)
    {
        carrotButton.interactable = interactable;
        tomatoButton.interactable = interactable;
        wheatButton.interactable = interactable;
        cornButton.interactable = interactable;
        grapeButton.interactable = interactable;
        potatoButton.interactable = interactable;
    }

    private void OnCorrectOrder()
    {
        Debug.Log("Correct!");

        correctYell.SetActive(true);
        Correct = true;

        int streak = PlayerPrefs.GetInt("PackingStreak", 0);
        streak++;
        PlayerPrefs.SetInt("PackingStreak", streak);
        PlayerPrefs.Save();

        CoinManager.Instance.AddCoins(10);
    }

    private void OnWrongOrder()
    {
        Debug.Log("Wrong!");

        wrongYell.SetActive(true);
        Correct = false;

        PlayerPrefs.SetInt("PackingStreak", 0);
    }

    private IEnumerator CloseMinigame()
    {
        yield return new WaitForSeconds(2.5f);
        DayManager.Instance.taskLeft = false;
        SceneManager.LoadScene("ShopScene");

        if (Correct == true)
        {
            NotificationManager.Instance.ShowNotification("You did well! The shopkeeper pays you 10 coins for your time", 3f);
        }
        else { NotificationManager.Instance.ShowNotification("You messed up! The shopkeeper is dissapointed and will do it himself", 3f); }

    }

}