using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public GameObject normalEggPrefab;
    public GameObject largeEggPrefab;
    public GameObject goldenEggPrefab;
    public Transform eggContainer; // Het panel waar eieren in spawnen

    void OnEnable()
    {
        SpawnSavedEggs();
    }

    private void SpawnSavedEggs()
    {
        int eggCount = PlayerPrefs.GetInt("eggs_to_spawn_count", 0);

        for (int i = 0; i < eggCount; i++)
        {
            string eggType = PlayerPrefs.GetString($"egg_{i}_type", "");
            float x = PlayerPrefs.GetFloat($"egg_{i}_x", 0);
            float y = PlayerPrefs.GetFloat($"egg_{i}_y", 0);

            GameObject eggPrefab = eggType switch
            {
                "normal" => normalEggPrefab,
                "large" => largeEggPrefab,
                "golden" => goldenEggPrefab,
                _ => normalEggPrefab
            };

            if (eggPrefab != null)
            {
                GameObject egg = Instantiate(eggPrefab, eggContainer);
                Egg eggScript = egg.GetComponent<Egg>();
                if (eggScript != null)
                {
                    eggScript.eggType = eggType;
                    eggScript.SetPosition(new Vector2(x, y));
                }
            }
        }
    }

    private void ClearEggSpawnData()
    {
        int eggCount = PlayerPrefs.GetInt("eggs_to_spawn_count", 0);

        for (int i = 0; i < eggCount; i++)
        {
            PlayerPrefs.DeleteKey($"egg_{i}_chickenID");
            PlayerPrefs.DeleteKey($"egg_{i}_type");
            PlayerPrefs.DeleteKey($"egg_{i}_x");
            PlayerPrefs.DeleteKey($"egg_{i}_y");
        }

        PlayerPrefs.SetInt("eggs_to_spawn_count", 0);
        PlayerPrefs.Save();
    }

}