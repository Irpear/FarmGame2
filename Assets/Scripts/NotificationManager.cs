using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// zo aanroepen:
// NotificationManager.Instance.ShowNotification("Not enough coins!");
// NotificationManager.Instance.ShowNotification("Wrong tool!", 1.5f);

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public CanvasGroup canvasGroup;  // Voeg dit toe aan je panel!

    public float displayDuration = 3f;
    public float fadeDuration = 0.4f;

    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private bool isShowingNotification = false;

    private struct NotificationData
    {
        public string message;
        public float duration;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        notificationPanel.SetActive(false);

        // Als je geen CanvasGroup hebt, voeg hem automatisch toe
        if (canvasGroup == null)
            canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
    }

    public void ShowNotification(string message, float duration = -1)
    {
        // Als geen duration gegeven, gebruik default
        if (duration < 0) duration = displayDuration;

        notificationQueue.Enqueue(new NotificationData { message = message, duration = duration });

        if (!isShowingNotification)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowingNotification = true;

        while (notificationQueue.Count > 0)
        {
            NotificationData data = notificationQueue.Dequeue();
            yield return StartCoroutine(ShowNotificationCoroutine(data.message, data.duration));
        }

        isShowingNotification = false;
    }

    private IEnumerator ShowNotificationCoroutine(string message, float duration)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        // Fade in
        yield return StartCoroutine(FadeTo(1f, fadeDuration));

        // Wacht
        yield return new WaitForSeconds(duration);

        // Fade out
        yield return StartCoroutine(FadeTo(0f, fadeDuration));

        canvasGroup.blocksRaycasts = false;
        notificationPanel.SetActive(false);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public bool IsShowing()
    {
        return notificationPanel.activeSelf;
    }
}