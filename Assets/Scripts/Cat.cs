using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cat : MonoBehaviour
{
    [Header("Cat Sprites")]
    public Sprite sleepingSprite;
    public Sprite awakeSprite;
    public Sprite happySprite;
    public Sprite angrySprite;

    [Header("References")]
    public Image catImage;
    public Button catButton;

    [Header("Timing Settings")]
    public float minPauseTime = 2f;
    public float maxPauseTime = 9f;
    public float reactionTime = 1f;

    [Header("Game Settings")]
    public int baseWakeUps = 3; // Start met 3 keer wakker worden

    public bool Correct;
    public AudioSource audioSource;
    public AudioClip angryCat;
    public AudioClip happyCat;

    private int catStreak
    {
        get => PlayerPrefs.GetInt("catStreak", 0);
        set { PlayerPrefs.SetInt("catStreak", value); PlayerPrefs.Save(); }
    }

    private int targetWakeUps;
    private int currentWakeUp;
    private bool isAwake = false;
    private bool gameActive = false;

    void Start()
    {
        targetWakeUps = baseWakeUps + catStreak;
        currentWakeUp = 0;

        catImage.sprite = sleepingSprite;
        catButton.onClick.AddListener(OnCatClicked);

        gameActive = true;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (currentWakeUp < targetWakeUps && gameActive)
        {
            // Pauze voordat kat wakker wordt
            float pauseTime = Random.Range(minPauseTime, maxPauseTime);
            yield return new WaitForSeconds(pauseTime);

            if (!gameActive) break;

            // Kat wordt wakker
            WakeCat();

            // Wacht op reactie of timeout
            float elapsed = 0f;
            bool clicked = false;

            while (elapsed < reactionTime && !clicked && gameActive)
            {
                elapsed += Time.deltaTime;
                yield return null;

                if (!isAwake) // Speler heeft geklikt
                {
                    clicked = true;
                    break;
                }
            }

            // Timeout - te laat geklikt
            if (!clicked && gameActive)
            {
                GameFailed();
                yield break;
            }

            currentWakeUp++;
        }

        // Alle wake-ups gehaald!
        if (gameActive)
        {
            GameCompleted();
        }
    }

    private void WakeCat()
    {
        isAwake = true;
        catImage.sprite = awakeSprite;
    }

    private void OnCatClicked()
    {
        if (!gameActive) return;

        if (isAwake)
        {
            // Correct - kat terug in slaap
            isAwake = false;
            catImage.sprite = sleepingSprite;
        }
        else
        {
            // Te vroeg geklikt - game failed
            GameFailed();
        }
    }

    private void GameCompleted()
    {
        gameActive = false;
        catImage.sprite = happySprite;

        Debug.Log("Cat correct!");

        audioSource.PlayOneShot(happyCat);
        Correct = true;

        catStreak++;

        CoinManager.Instance.AddCoins(10);
        StartCoroutine(CloseMinigame());
    }

    private void GameFailed()
    {
        gameActive = false;
        isAwake = false;
        catImage.sprite = angrySprite;
        catStreak = 0;

        audioSource.PlayOneShot(angryCat);
        Correct = false;

        Debug.Log("Cat game failed! Streak reset.");
        StartCoroutine(CloseMinigame());
    }

    private IEnumerator CloseMinigame()
    {
        yield return new WaitForSeconds(2.5f);
        DayManager.Instance.taskLeft = false;
        SceneManager.LoadScene("ShopScene");

        if (Correct == true)
        {
            NotificationManager.Instance.ShowNotification("You did well! The shopkeeper pays you 10 coins for your effort", 3f);
            FindAnyObjectByType<MinigameController>()?.TaskCompleted();
        }
        else { NotificationManager.Instance.ShowNotification("You messed up! The shopkeeper is disappointed and afraid", 3f); }

    }

}