using UnityEngine;
using UnityEngine.UI;

public class Egg : MonoBehaviour
{
    public string eggType; // "normal", "large", "golden"
    public int eggValue; // Waarde bij verkoop

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void CollectEgg()
    {
        // Verkoop ei
        int value = eggType switch
        {
            "normal" => 10,
            "large" => 25,
            "golden" => 100,
            _ => 5
        };

        CoinManager.Instance.AddCoins(value);
        NotificationManager.Instance.ShowNotification($"Collected {eggType} egg! +{value} coins");

        int count = PlayerPrefs.GetInt("eggs_to_spawn_count", 0);
        if (count > 0) count--;
        PlayerPrefs.SetInt("eggs_to_spawn_count", count);

        if (PlayerPrefs.GetInt("feeder2_available", 0) == 0)
        {
            PlayerPrefs.SetInt("feeder2_available", 1);
            NotificationManager.Instance.ShowNotification("More items have been unlocked at the store");
        }
            PlayerPrefs.Save();

        Destroy(gameObject);
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }
}