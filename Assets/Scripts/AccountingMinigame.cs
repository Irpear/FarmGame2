using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountingMinigame : MonoBehaviour
{
    [Header("Income Fields")]
    public TextMeshProUGUI income1;
    public TextMeshProUGUI income2;
    public TextMeshProUGUI income3;
    public TextMeshProUGUI income4;
    public TextMeshProUGUI totalIncome;

    [Header("Expense Fields")]
    public TextMeshProUGUI expense1;
    public TextMeshProUGUI expense2;
    public TextMeshProUGUI expense3;
    public TextMeshProUGUI expense4;
    public TextMeshProUGUI totalExpense;

    [Header("Input & Button")]
    public TMP_InputField profitInput;
    public Button signButton;

    [Header("Number Ranges")]
    public int incomeMin = 400;
    public int incomeMax = 1000;
    public int expenseMin = 50;
    public int expenseMax = 400;

    [Header("Timer")]
    public float timeLimit = 15f;

    [Header("The rest")]
    public GameObject minigamePanel;
    public GameObject sign;
    public GameObject wrongStamp;
    public GameObject correctStamp;
    public bool Correct;
    public TextMeshProUGUI Timer;

    [Header("Streak Settings")]
    public int maxStreak = 14;
    public float timePenaltyPerStreak = 1f;
    public float minimumTimeLimit = 1f;

    private int calculatedProfit;
    private float timeRemaining;
    private bool done = false;

    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition;
    private bool isKeyboardOpen = false;

    void Start()
    {
        SetupInputField();
        GenerateNumbers();
        signButton.onClick.AddListener(CheckAnswer);

        int streak = PlayerPrefs.GetInt("AccountingStreak", 0);
        streak = Mathf.Clamp(streak, 0, maxStreak);

        float adjustedTime = timeLimit - (streak * timePenaltyPerStreak);
        timeRemaining = Mathf.Max(adjustedTime, minimumTimeLimit);

        Debug.Log($"Accounting streak: {streak}, time limit: {timeRemaining}");

        timeLimit = adjustedTime;

        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        originalPosition = canvasRect.anchoredPosition;
    }

    void Update()
    {
        // Timer countdown
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            Timer.text = timeRemaining.ToString("F1");

            if (timeRemaining <= 0 && done == false)
            {
                TimeUp();
            }
        }
        HandleKeyboard();
    }

    private void SetupInputField()
    {
        // Zet input field op numeriek keyboard voor mobiel
        profitInput.contentType = TMP_InputField.ContentType.IntegerNumber;
        profitInput.characterLimit = 6; // Max 6 cijfers
        profitInput.text = ""; // Start leeg
    }

    private void GenerateNumbers()
    {
        // Genereer incomes
        int inc1 = Random.Range(incomeMin, incomeMax + 1);
        int inc2 = Random.Range(incomeMin, incomeMax + 1);
        int inc3 = Random.Range(incomeMin, incomeMax + 1);
        int inc4 = Random.Range(incomeMin, incomeMax + 1);

        income1.text = inc1.ToString();
        income2.text = inc2.ToString();
        income3.text = inc3.ToString();
        income4.text = inc4.ToString();

        int totalInc = inc1 + inc2 + inc3 + inc4;
        totalIncome.text = totalInc.ToString();

        // Genereer expenses
        int exp1 = Random.Range(expenseMin, expenseMax + 1);
        int exp2 = Random.Range(expenseMin, expenseMax + 1);
        int exp3 = Random.Range(expenseMin, expenseMax + 1);
        int exp4 = Random.Range(expenseMin, expenseMax + 1);

        expense1.text = exp1.ToString();
        expense2.text = exp2.ToString();
        expense3.text = exp3.ToString();
        expense4.text = exp4.ToString();

        int totalExp = exp1 + exp2 + exp3 + exp4;
        totalExpense.text = totalExp.ToString();

        // Bereken correcte profit
        calculatedProfit = totalInc - totalExp;

        Debug.Log($"Correct profit: {calculatedProfit}");
    }

    private void CheckAnswer()
    {

        sign.SetActive(true);
        done = true;
        signButton.interactable = false;

        // Probeer input te parsen
        if (int.TryParse(profitInput.text, out int playerAnswer))
        {
            if (playerAnswer == calculatedProfit)
            {
                OnCorrectAnswer();
            }
            else
            {
                OnWrongAnswer();
            }
        }
        else
        {
            OnWrongAnswer(); // Geen geldig getal ingevoerd
        }
        StartCoroutine(CloseMinigame());
    }

    private void TimeUp()
    {
        timeRemaining = 0;

        Debug.Log("Time's up!");

        wrongStamp.SetActive(true);
        Correct = false;

        PlayerPrefs.SetInt("AccountingStreak", 0);

        StartCoroutine(CloseMinigame());
        signButton.interactable = false;
    }

    private void OnCorrectAnswer()
    {
        // VOEG HIER JE CODE TOE VOOR CORRECT ANTWOORD
        Debug.Log("Correct!");

        correctStamp.SetActive(true);
        Correct = true;

        int streak = PlayerPrefs.GetInt("AccountingStreak", 0);
        streak++;
        PlayerPrefs.SetInt("AccountingStreak", streak);
        PlayerPrefs.Save();

        CoinManager.Instance.AddCoins(10);
    }

    private void OnWrongAnswer()
    {
        // VOEG HIER JE CODE TOE VOOR FOUT ANTWOORD
        Debug.Log("Wrong!");

        wrongStamp.SetActive(true);
        Correct = false;

        PlayerPrefs.SetInt("AccountingStreak", 0);
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public int GetCorrectProfit()
    {
        return calculatedProfit;
    }

    private IEnumerator CloseMinigame()
    {
        yield return new WaitForSeconds(2.5f);
        //sign.SetActive(false);
        //wrongStamp.SetActive(false);
        //correctStamp.SetActive(false);
        //minigamePanel.SetActive(false);
        DayManager.Instance.taskLeft = false;
        SceneManager.LoadScene("ShopScene");

        if (Correct == true)
        {
            NotificationManager.Instance.ShowNotification("You were correct! The shopkeeper pays you 10 coins for your time", 3f);
        }
        else { NotificationManager.Instance.ShowNotification("Wrong answer! The shopkeeper can't count on you and will do it himself", 3f); }

    }

    private void HandleKeyboard()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (TouchScreenKeyboard.visible && !isKeyboardOpen)
        {
            // Keyboard net geopend
            isKeyboardOpen = true;
            float keyboardHeight = GetKeyboardHeight();
            canvasRect.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
        }
        else if (!TouchScreenKeyboard.visible && isKeyboardOpen)
        {
            // Keyboard net gesloten
            isKeyboardOpen = false;
            canvasRect.anchoredPosition = originalPosition;
        }
#endif
    }

    private float GetKeyboardHeight()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Schat keyboard hoogte (ongeveer 40% van scherm hoogte)
        return Screen.height * 0.4f;
#else
        return 0f;
#endif
    }
}