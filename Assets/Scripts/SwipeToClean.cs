using UnityEngine;
using UnityEngine.UI;

public class SwipeToClean : MonoBehaviour
{
    public float brushSize = 40f;
    public float cleanPercentToWin = 0.85f;

    private Texture2D dirtTexture;
    private Color[] originalPixels;
    private RectTransform rectTransform;
    private bool finished = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Image img = GetComponent<Image>();
        Sprite sprite = img.sprite;

        dirtTexture = Instantiate(sprite.texture);
        dirtTexture.Apply();

        img.sprite = Sprite.Create(
            dirtTexture,
            sprite.rect,
            new Vector2(0.5f, 0.5f),
            sprite.pixelsPerUnit
        );

        originalPixels = dirtTexture.GetPixels();
    }

    void Update()
    {
        if (finished) return;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
            EraseAt(Input.mousePosition);
#else
        if (Input.touchCount > 0)
            EraseAt(Input.GetTouch(0).position);
#endif
    }

    void EraseAt(Vector2 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPos, null, out localPoint);

        Rect rect = rectTransform.rect;

        float x = (localPoint.x - rect.x) / rect.width;
        float y = (localPoint.y - rect.y) / rect.height;

        int px = Mathf.RoundToInt(x * dirtTexture.width);
        int py = Mathf.RoundToInt(y * dirtTexture.height);

        EraseCircle(px, py);
        dirtTexture.Apply();

        if (GetCleanPercent() >= cleanPercentToWin)
        {
            finished = true;
            OnCleaned();
        }
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

    void OnCleaned()
    {
        Debug.Log("Floor cleaned!");

        // hier roep je je win logic aan
        FindAnyObjectByType<MinigameController>()?.TaskCompleted();

        // bijv:
        // correctStamp.SetActive(true);
        // StartCoroutine(CloseMinigame());
    }
}
