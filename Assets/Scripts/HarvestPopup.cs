using UnityEngine;
using TMPro;

public class HarvestPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup group;
    public float riseDistance = 50f;
    public float duration = 1f;
    private float timer;

    public void Show(int amount, Vector3 startPos)
    {
        transform.position = startPos;
        text.text = "+" + amount;
        group.alpha = 1;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        transform.position += Vector3.up * (riseDistance * Time.deltaTime);
        group.alpha = 1 - (timer / duration);

        if (timer >= duration)
            Destroy(gameObject);
    }
}
