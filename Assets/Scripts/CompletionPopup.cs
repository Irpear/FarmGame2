using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Popup als alle trofeeën behaald zijn: felicitatie, aantal dagen, screenshot opslaan,
// delen, opnieuw beginnen (met bevestiging) of doorgaan met eindeloos farmen.
//
// Staat op een eigen canvas in de FarmScene en blijft tussen scenes bestaan,
// zodat de popup overal kan verschijnen (ook in de shop of schuur).
//
// Opslaan in de galerij en delen gebeurt met de plugins NativeGallery en NativeShare (yasirkula).

public class CompletionPopup : MonoBehaviour
{
    public static CompletionPopup Instance;

    [Header("Popup")]
    public GameObject popupPanel;    // staat uit, gaat aan bij uitspelen
    public Text daysText;            // "You completed Farmhand in X days!"
    public GameObject buttonsGroup;  // knoppen, worden verborgen tijdens de screenshot

    [Header("Buttons")]
    public Button saveScreenshotButton;
    public Button shareButton;
    public Button restartButton;
    public Button continueButton;

    [Header("Restart confirmation")]
    public GameObject confirmPanel;  // "Are you sure?" scherm, staat uit
    public Button confirmYesButton;
    public Button confirmNoButton;

    [Header("Settings")]
    public string gameName = "Farmhand";

    private const string CompletedDayKey = "game_completed_day";   // 0 = nog niet uitgespeeld
    private const string PopupSeenKey = "completion_popup_seen";

    private bool busy = false; // screenshot bezig

    public static bool IsOpen => Instance != null && Instance.popupPanel != null && Instance.popupPanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveScreenshotButton.onClick.AddListener(OnSaveScreenshot);
        shareButton.onClick.AddListener(OnShare);
        restartButton.onClick.AddListener(OnRestart);
        continueButton.onClick.AddListener(OnContinue);
        confirmYesButton.onClick.AddListener(OnConfirmRestart);
        confirmNoButton.onClick.AddListener(OnCancelRestart);

        popupPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    private void Start()
    {
        if (Instance != this) return;

        // App afgesloten terwijl de popup nog open stond? Dan opnieuw tonen.
        if (IsCompleted() && PlayerPrefs.GetInt(PopupSeenKey, 0) == 0)
            StartCoroutine(ShowWhenReady());
    }

    // ================= UITSPELEN =================

    public static bool IsCompleted()
    {
        return PlayerPrefs.GetInt(CompletedDayKey, 0) > 0;
    }

    // Aangeroepen door Trophies zodra de laatste trofee binnen is
    public static void OnGameCompleted()
    {
        if (IsCompleted()) return;

        PlayerPrefs.SetInt(CompletedDayKey, DayManager.Instance.currentDay);
        PlayerPrefs.Save();

        if (Instance != null)
            Instance.StartCoroutine(Instance.ShowWhenReady());
    }

    // Wacht tot de trofee-melding weg is en het geen nacht is
    private IEnumerator ShowWhenReady()
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
        }
        while ((NotificationManager.Instance != null && NotificationManager.Instance.IsShowing())
            || (DayManager.Instance != null && DayManager.Instance.isEndingDay));

        Show();
    }

    private void Show()
    {
        int days = PlayerPrefs.GetInt(CompletedDayKey, 0);
        daysText.text = $"You completed {gameName} in {DaysString(days)}!";

        confirmPanel.SetActive(false);
        buttonsGroup.SetActive(true);
        popupPanel.SetActive(true);
    }

    // Testen: in Play mode rechtsklik op dit component in de Inspector → "Test: show popup"
    [ContextMenu("Test: show popup")]
    private void TestShow()
    {
        PlayerPrefs.SetInt(CompletedDayKey, DayManager.Instance.currentDay);
        Show();
    }

    private string DaysString(int days)
    {
        return days == 1 ? "1 day" : $"{days} days";
    }

    // ================= SCREENSHOT =================

    private void OnSaveScreenshot()
    {
        if (busy) return;

        StartCoroutine(CapturePopup(png =>
        {
            string fileName = $"{gameName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            NativeGallery.SaveImageToGallery(png, gameName, fileName, (success, path) =>
            {
                if (success) NotificationManager.Instance.ShowNotification("Screenshot saved!");
                else NotificationManager.Instance.ShowNotification("Could not save the screenshot");
            });
        }));
    }

    private void OnShare()
    {
        if (busy) return;

        StartCoroutine(CapturePopup(png =>
        {
            string path = Path.Combine(Application.temporaryCachePath, "completed.png");
            File.WriteAllBytes(path, png);

            int days = PlayerPrefs.GetInt(CompletedDayKey, 0);

            new NativeShare()
                .AddFile(path)
                .SetText($"I completed {gameName} in {DaysString(days)}")
                .Share();
        }));
    }

    // Maakt een foto van het scherm met de popup erop, zonder de knoppen
    private IEnumerator CapturePopup(Action<byte[]> onCaptured)
    {
        busy = true;
        buttonsGroup.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] png = screenshot.EncodeToPNG();
        Destroy(screenshot);

        buttonsGroup.SetActive(true);
        busy = false;

        onCaptured(png);
    }

    // ================= OPNIEUW / DOORGAAN =================

    private void OnRestart()
    {
        confirmPanel.SetActive(true);
    }

    private void OnCancelRestart()
    {
        confirmPanel.SetActive(false);
    }

    private void OnConfirmRestart()
    {
        confirmPanel.SetActive(false);
        popupPanel.SetActive(false);
        SaveManager.ResetAllProgress();
    }

    private void OnContinue()
    {
        PlayerPrefs.SetInt(PopupSeenKey, 1);
        PlayerPrefs.Save();
        popupPanel.SetActive(false);
    }
}
