using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NightTransition : MonoBehaviour
{
    public Image blackPanel;
    public float fadeDuration = 1f;

    private void Awake()
    {
        blackPanel.color = new Color(0, 0, 0, 0);
        gameObject.SetActive(true);
    }

    public void PlayTransition(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(onComplete));
    }

    private IEnumerator FadeCoroutine(System.Action onComplete)
    {

        blackPanel.raycastTarget = true;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackPanel.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }
        blackPanel.color = Color.black;

        // Eventueel korte pause
        yield return new WaitForSeconds(0.2f);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackPanel.color = new Color(0, 0, 0, 1 - t / fadeDuration);
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);

        blackPanel.raycastTarget = false;


        onComplete?.Invoke();
    }
}