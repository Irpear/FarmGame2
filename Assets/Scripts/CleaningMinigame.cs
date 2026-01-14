using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleaningMinigame : MonoBehaviour
{
    public float brushSize = 100f;
    public float cleanPercentToWin = 0.95f;

    private Texture2D dirtTexture;
    private Color[] originalPixels;
    private RectTransform rectTransform;
    private bool finished = false;

    public Image Dirt;

    private Canvas canvas;

    [Header("Timer")]
    public float timeLimit = 10f;

    [Header("The rest")]
    public GameObject minigamePanel;
    public bool Correct;
    public TextMeshProUGUI Timer;

    public GameObject wrongYell;
    public GameObject correctYell;
    public AudioSource audioSource;
    public AudioClip angryMan;
    public AudioClip happyMan;

    [Header("Streak Settings")]
    public int maxStreak = 9;
    public float timePenaltyPerStreak = 1f;
    public float minimumTimeLimit = 1f;

    private float timeRemaining;

    private Vector2? lastErasePos = null;

    private bool rewardGiven = false;




    void Start()
    {
        rectTransform = Dirt.GetComponent<RectTransform>();

        Sprite sprite = Dirt.sprite;

        dirtTexture = Instantiate(sprite.texture);
        dirtTexture.Apply();

        Dirt.sprite = Sprite.Create(
            dirtTexture,
            sprite.rect,
            new Vector2(0.5f, 0.5f),
            sprite.pixelsPerUnit
        );

        originalPixels = dirtTexture.GetPixels();

        canvas = Dirt.canvas;

        int streak = PlayerPrefs.GetInt("CleaningStreak", 0);
        streak = Mathf.Clamp(streak, 0, maxStreak);

        float adjustedTime = timeLimit - (streak * timePenaltyPerStreak);
        timeRemaining = Mathf.Max(adjustedTime, minimumTimeLimit);

        Debug.Log($"Cleaning streak: {streak}, time limit: {timeRemaining}");

        timeLimit = adjustedTime;
    }

    void Update()
    {
        if (finished) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            Timer.text = timeRemaining.ToString("F1");

            if (timeRemaining <= 0 && finished == false)
            {
                TimeUp();   
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            EraseLine(Input.mousePosition);
        }
        else
        {
            lastErasePos = null;
        }
#else
if (Input.touchCount > 0)
{
    EraseLine(Input.GetTouch(0).position);
}
else
{
    lastErasePos = null;
}
#endif

    }

    void EraseAt(Vector2 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPos, canvas.worldCamera, out localPoint);

        Rect rect = rectTransform.rect;

        if (!rect.Contains(localPoint))
            return;


        float x = (localPoint.x - rect.x) / rect.width;
        float y = (localPoint.y - rect.y) / rect.height;

        int px = Mathf.RoundToInt(x * dirtTexture.width);
        int py = Mathf.RoundToInt(y * dirtTexture.height);

        EraseCircle(px, py);
        dirtTexture.Apply();

        if (GetCleanPercent() >= cleanPercentToWin)
        {
            finished = true;
            OnClean();
        }
    }

    void EraseLine(Vector2 screenPos)
    {
        if (lastErasePos == null)
        {
            EraseAt(screenPos);
            lastErasePos = screenPos;
            return;
        }

        Vector2 prev = lastErasePos.Value;
        float dist = Vector2.Distance(prev, screenPos);
        int steps = Mathf.Max(1, Mathf.CeilToInt(dist / (brushSize * 0.5f)));

        for (int i = 0; i <= steps; i++)
        {
            Vector2 p = Vector2.Lerp(prev, screenPos, i / (float)steps);
            EraseAt(p);
        }

        lastErasePos = screenPos;
    }


    void EraseCircle(int cx, int cy)
    {
        int r = Mathf.RoundToInt(brushSize);

        for (int x = -r; x < r; x++)
        {
            for (int y = -r; y < r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < dirtTexture.width &&
                        py >= 0 && py < dirtTexture.height)
                    {
                        dirtTexture.SetPixel(px, py, Color.clear);
                    }
                }
            }
        }
    }

    float GetCleanPercent()
    {
        Color[] pixels = dirtTexture.GetPixels();
        int clear = 0;

        foreach (var p in pixels)
        {
            if (p.a < 0.1f) clear++;
        }

        return (float)clear / pixels.Length;
    }

    private void TimeUp()
    {
        timeRemaining = 0;

        if (finished) return;

        finished = true;

        Debug.Log("Time's up!");

        wrongYell.SetActive(true);
        Correct = false;

        PlayerPrefs.SetInt("CleaningStreak", 0);

        StartCoroutine(CloseMinigame());
    }

    private void OnClean()
    {
        // VOEG HIER JE CODE TOE VOOR CORRECT ANTWOORD
        Debug.Log("Correct!");

        correctYell.SetActive(true);
        Correct = true;

        int streak = PlayerPrefs.GetInt("CleaningStreak", 0);
        streak++;
        PlayerPrefs.SetInt("CleaningStreak", streak);
        PlayerPrefs.Save();

        if (rewardGiven) return;
        else
        {
            CoinManager.Instance.AddCoins(10);
            rewardGiven = true;
            StartCoroutine(CloseMinigame());
        }
    }

    private IEnumerator CloseMinigame()
    {
        yield return new WaitForSeconds(2.5f);
        DayManager.Instance.taskLeft = false;
        SceneManager.LoadScene("ShopScene");

        if (Correct == true)
        {
            NotificationManager.Instance.ShowNotification("You did well! The shopkeeper pays you 10 coins for your time", 3f);
            FindAnyObjectByType<MinigameController>()?.TaskCompleted();
        }
        else { NotificationManager.Instance.ShowNotification("The floor is still dirty! The shopkeeper will do it himself", 3f); }

    }
}