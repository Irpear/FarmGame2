using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Speelt om de zoveel seconden (willekeurig tussen min en max) een korte animatie af, bv. zwaaien.
// Werkt op een UI Image of op een SpriteRenderer, wat er op het object zit.
public class Shopkeeper : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite idleSprite;          // stilstaand, leeg laten = huidige sprite
    public Sprite[] waveFrames;        // zwaai-animatie, in volgorde
    public float frameDuration = 0.1f; // seconden per frame

    [Header("Timing (seconden)")]
    public float minInterval = 2f;
    public float maxInterval = 6f;

    private Image image;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (idleSprite == null)
            idleSprite = image != null ? image.sprite : spriteRenderer != null ? spriteRenderer.sprite : null;
    }

    // OnEnable zodat hij ook weer start als zijn panel opnieuw aangaat
    void OnEnable()
    {
        SetSprite(idleSprite);
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            foreach (var frame in waveFrames)
            {
                SetSprite(frame);
                yield return new WaitForSeconds(frameDuration);
            }

            SetSprite(idleSprite);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (image != null) image.sprite = sprite;
        else if (spriteRenderer != null) spriteRenderer.sprite = sprite;
    }
}
