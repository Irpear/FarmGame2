using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// Intro bij het opstarten: speelt een video met een los geluidsfragment af,
// fade vlak voor het einde naar zwart en fade daarna vanuit zwart de farm in.
// Staat in de IntroScene, die als eerste scene in de build staat.
//
// Het script maakt zelf een scherm-vullend plaatje (RawImage) waarop de video komt,
// zodat de video gedraaid kan worden. Telefoonvideo's zijn vaak liggend opgeslagen met
// een "draai 90°"-notitie, die Unity negeert. Video Rotation zet dat recht.
public class IntroPlayer : MonoBehaviour
{
    [Header("Intro")]
    public VideoClip introVideo;
    public AudioClip introSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Beeld")]
    [Tooltip("Graden om de video te draaien: -90 = kwartslag met de klok mee, 90 = tegen de klok in, 0 = niet draaien")]
    public float videoRotation = -90f;

    [Header("Timing")]
    [Tooltip("Hoeveel seconden het geluid eerder start dan de video. Negatief = geluid start later.")]
    [Range(-2f, 2f)] public float audioLead = 0f;

    [Header("Overgang naar het spel")]
    [Tooltip("Zoveel seconden vóór het einde van de video begint het naar zwart te faden")]
    public float fadeStartBeforeEnd = 0.4f;
    [Tooltip("Duur van video → zwart")]
    public float fadeToBlackDuration = 0.4f;
    [Tooltip("Duur van zwart → spel")]
    public float fadeInDuration = 0.8f;
    public string nextScene = "FarmScene";

    [Header("Testen (alleen in de editor)")]
    [Tooltip("Aan: de intro herhaalt steeds in plaats van naar de farm te gaan. Schuif tijdens Play met Audio Lead en luister. Werkt nooit in een build.")]
    public bool loopForTesting = false;

    [Header("Veiligheid")]
    public float prepareTimeout = 5f; // video laadt niet? dan na zoveel seconden toch door

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private GameObject introCanvas;
    private RawImage videoImage;
    private Image blackOverlay;
    private RenderTexture renderTexture;
    private AsyncOperation loading;
    private bool videoDone = false;

    void Awake()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.clip = introVideo;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // geluid komt uit het losse fragment
        videoPlayer.loopPointReached += _ => videoDone = true;
        videoPlayer.errorReceived += (_, message) =>
        {
            Debug.LogWarning("Intro video error: " + message);
            videoDone = true;
        };

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = introSound;
        audioSource.volume = soundVolume;

        CreateCanvas();
    }

    // Eigen canvas met de video en een zwarte laag erover, zodat er in de scene niets extra's nodig is
    private void CreateCanvas()
    {
        introCanvas = new GameObject("IntroCanvas", typeof(Canvas));
        Canvas canvas = introCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // boven alles, ook straks in de farm

        GameObject videoObj = new GameObject("IntroVideo", typeof(RawImage));
        videoObj.transform.SetParent(introCanvas.transform, false);
        videoImage = videoObj.GetComponent<RawImage>();
        videoImage.raycastTarget = false;
        videoImage.enabled = false; // pas aan als de video start

        GameObject blackObj = new GameObject("BlackOverlay", typeof(Image));
        blackObj.transform.SetParent(introCanvas.transform, false);
        blackOverlay = blackObj.GetComponent<Image>();
        blackOverlay.color = new Color(0f, 0f, 0f, 0f);
        RectTransform rt = blackOverlay.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    IEnumerator Start()
    {
        // Farm alvast op de achtergrond laden, pas wisselen als de intro klaar is
        loading = SceneManager.LoadSceneAsync(nextScene);
        loading.allowSceneActivation = false;

        while (true)
        {
            yield return PlayIntroOnce();
            yield return FadeOverlay(0f, 1f, fadeToBlackDuration, true);

            if (!(Application.isEditor && loopForTesting)) break;

            // Testmodus: even zwart, dan opnieuw (met de huidige Audio Lead)
            ResetForReplay();
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Zwart scherm meenemen naar de farm en daar wegfaden
        videoPlayer.Stop();
        audioSource.Stop();
        videoImage.enabled = false;
        DontDestroyOnLoad(introCanvas);
        DontDestroyOnLoad(gameObject);

        loading.allowSceneActivation = true;
        while (!loading.isDone)
            yield return null;

        // Paar frames wachten zodat het opstarten van de farm onder het zwart gebeurt
        yield return null;
        yield return null;

        yield return FadeOverlay(1f, 0f, fadeInDuration, false);

        Destroy(introCanvas);
        Destroy(gameObject);
    }

    private IEnumerator PlayIntroOnce()
    {
        if (introVideo != null)
        {
            // Eerst voorbereiden, zodat beeld en geluid precies op het gewenste moment beginnen
            videoPlayer.Prepare();
            float waited = 0f;
            while (!videoPlayer.isPrepared && !videoDone && waited < prepareTimeout)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }

            if (videoPlayer.isPrepared)
            {
                if (renderTexture == null)
                    SetupVideoImage((int)videoPlayer.width, (int)videoPlayer.height);

                if (audioLead > 0f)
                {
                    // Geluid eerst, video iets later
                    if (introSound != null) audioSource.Play();
                    yield return new WaitForSecondsRealtime(audioLead);
                    StartVideo();
                }
                else
                {
                    // Video eerst (of tegelijk), geluid iets later
                    StartVideo();
                    if (introSound != null) audioSource.PlayScheduled(AudioSettings.dspTime - audioLead);
                }

                // Wachten tot vlak voor het einde, dan begint de fade naar zwart
                while (!videoDone && videoPlayer.length - videoPlayer.time > fadeStartBeforeEnd)
                    yield return null;
            }
        }
        else if (introSound != null)
        {
            // Geen video ingesteld: alleen het geluid afspelen
            audioSource.Play();
            yield return new WaitForSeconds(Mathf.Max(0f, introSound.length - fadeStartBeforeEnd));
        }
    }

    private void StartVideo()
    {
        videoImage.enabled = true;
        videoPlayer.Play();
    }

    private void ResetForReplay()
    {
        videoPlayer.Stop();
        audioSource.Stop();
        audioSource.volume = soundVolume;
        videoImage.enabled = false;
        blackOverlay.color = new Color(0f, 0f, 0f, 0f);
        videoDone = false;
    }

    private void SetupVideoImage(int width, int height)
    {
        renderTexture = new RenderTexture(width, height, 0);
        videoPlayer.targetTexture = renderTexture;

        videoImage.texture = renderTexture;

        RectTransform rt = videoImage.rectTransform;
        rt.localRotation = Quaternion.Euler(0f, 0f, videoRotation);

        // Na een kwartslag draaien zijn breedte en hoogte op het scherm omgewisseld
        bool sideways = Mathf.Abs(Mathf.DeltaAngle(videoRotation, 0f)) > 45f
                     && Mathf.Abs(Mathf.DeltaAngle(videoRotation, 180f)) > 45f;
        float shownWidth = sideways ? height : width;
        float shownHeight = sideways ? width : height;

        // Zo groot mogelijk, hele video zichtbaar (zwarte randen waar nodig)
        // (Overlay canvas zonder scaler: 1 eenheid = 1 pixel)
        float scale = Mathf.Min(Screen.width / shownWidth, Screen.height / shownHeight);
        rt.sizeDelta = new Vector2(width * scale, height * scale);
    }

    // Zwarte laag van 'from' naar 'to' (0 = doorzichtig, 1 = zwart), eventueel met het geluid mee uitfaden
    private IEnumerator FadeOverlay(float from, float to, float duration, bool fadeAudio)
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            blackOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, p));
            if (fadeAudio) audioSource.volume = startVolume * (1f - p);
            yield return null;
        }

        blackOverlay.color = new Color(0f, 0f, 0f, to);
        if (fadeAudio) audioSource.volume = 0f;
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
