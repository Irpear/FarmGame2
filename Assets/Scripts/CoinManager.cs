using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coins = 5;
    public int profit = 0;
    public TextMeshProUGUI coinText;

    public int wheatResource = 0;
    public int cornResource = 0;

    public int animalFood = 0;
    public int animalFood2 = 0;

    private void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Update UI wanneer scene verandert
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Zoek de coin UI in de nieuwe scene als die opnieuw instantiëert
        if (coinText == null)
            coinText = GameObject.Find("CoinText")?.GetComponent<TextMeshProUGUI>();

        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        if (amount > 0) profit += amount;
        coins += amount;
        UpdateUI();
    }

    public void AddWheat(int amount)
    {
        wheatResource += amount;
        UpdateUI();
    }

    public void AddAnimalFood(int amount)
    {
        animalFood += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}