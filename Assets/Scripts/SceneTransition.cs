using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public Image fadeImage; // Zwarte Image over hele scherm
    public float fadeDuration = 0.7f;

    public float fadeOutDuration = 0.5f; // Langer (naar zwart)
    public float fadeInDuration = 0.2f;  // Korter (van zwart)

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
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade out (zwart) - langzaam
        yield return StartCoroutine(Fade(1f, fadeOutDuration));

        // Wacht extra frame zodat scene checks klaar zijn
        yield return new WaitForSeconds(0.1f);

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wacht nog een frame
        yield return null;

        // Fade in (transparant) - snel
        yield return StartCoroutine(Fade(0f, fadeInDuration));
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        Color c = fadeImage.color;
        float startAlpha = c.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        fadeImage.color = c;
    }

    public void SwitchPanels(System.Action switchAction)
    {
        StartCoroutine(FadeAndSwitchPanels(switchAction));
    }

    private IEnumerator FadeAndSwitchPanels(System.Action switchAction)
    {
        // Kort naar zwart
        yield return StartCoroutine(Fade(1f, 0.15f));

        // Switch panels
        switchAction?.Invoke();

        // Kort terug
        yield return StartCoroutine(Fade(0f, 0.15f));
    }
}