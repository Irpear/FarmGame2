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
        // Eieren van een vorige keer openen weghalen, anders staan ze er dubbel
        foreach (var oldEgg in eggContainer.GetComponentsInChildren<Egg>(true))
        {
            Destroy(oldEgg.gameObject);
        }

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
                    eggScript.spawner = this;
                    eggScript.SetPosition(new Vector2(x, y));
                }
            }
        }
    }

    // Schrijft de eieren die nog liggen opnieuw weg, zodat precies het geraapte ei verdwijnt
    public void SaveRemainingEggs()
    {
        int count = 0;

        foreach (var egg in eggContainer.GetComponentsInChildren<Egg>(true))
        {
            if (egg.collected) continue;

            Vector2 pos = egg.GetPosition();
            PlayerPrefs.SetString($"egg_{count}_type", egg.eggType);
            PlayerPrefs.SetFloat($"egg_{count}_x", pos.x);
            PlayerPrefs.SetFloat($"egg_{count}_y", pos.y);
            count++;
        }

        PlayerPrefs.SetInt("eggs_to_spawn_count", count);
        PlayerPrefs.Save();
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
