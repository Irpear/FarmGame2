using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Stapel vuilnis die je kunt opruimen voor coins. Je krijgt er iets voor terug en daarna is hij voorgoed weg.
// Zet dit op een UI-object met een Image (de Button wordt automatisch toegevoegd).
// Elke stapel moet een eigen Pile ID hebben, daarmee wordt onthouden dat hij opgeruimd is.
// Let op: de Pile ID's tellen mee voor de trofee "koop alles" (zie Trophies.CheckBuyEverything).
//
// Volgorde bij opruimen: betalen → animatie + geluid → rewards + melding + zwevende popups → weg

[RequireComponent(typeof(Button))]
public class TrashPile : MonoBehaviour
{
    // Namen gelijk aan de seedTypes in de rest van de game
    public enum SeedType { carrot, tomato, wheat, corn, grape, potato }

    [System.Serializable]
    public struct SeedReward
    {
        public SeedType seedType;
        public int amount;
    }

    [Header("Settings")]
    public string pileID = "trashPile_1"; // uniek per stapel!
    public int cost = 50;

    [Header("Rewards")]
    public int rewardCoins = 0;
    public int rewardAnimalFood = 0;
    public int rewardAnimalFood2 = 0;
    public int rewardWheat = 0;
    public int rewardCorn = 0;
    public SeedReward[] rewardSeeds;

    [Header("UI")]
    public Text costText;
    public string costTextFormat = "Spend {0} coins to clean up"; // {0} = kosten
    public string notificationText = "You cleaned up the trash and found something useful!";

    [Header("Cleanup animation")]
    public Sprite[] cleanupFrames;       // vervangen na elkaar de vuilnis-sprite
    public float frameDuration = 0.1f;   // seconden per frame
    public AudioClip cleanupSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Reward popups (optioneel)")]
    public HarvestPopup coinPopupPrefab;  // HarvestPopup prefab
    public HarvestPopup wheatPopupPrefab; // WheatPopup prefab
    public HarvestPopup cornPopupPrefab;  // CornPopup prefab
    public float popupSpacing = 80f;      // afstand tussen popups naast elkaar

    private Image image;
    private bool cleaning = false;

    private bool Cleaned
    {
        get => PlayerPrefs.GetInt(pileID, 0) == 1;
        set { PlayerPrefs.SetInt(pileID, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    void Awake()
    {
        if (Cleaned)
        {
            gameObject.SetActive(false);
            return;
        }

        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(CleanUp);

        if (costText != null)
            costText.text = string.Format(costTextFormat, cost);
    }

    public void CleanUp()
    {
        if (cleaning || Cleaned) return;

        if (CoinManager.Instance.coins < cost)
        {
            NotificationManager.Instance.ShowNotification($"You need {cost} coins!");
            return;
        }

        cleaning = true;
        CoinManager.Instance.AddCoins(-cost);

        StartCoroutine(CleanUpRoutine());
    }

    private IEnumerator CleanUpRoutine()
    {
        if (costText != null) costText.gameObject.SetActive(false);

        // Geluid op de camera afspelen, dan loopt het door als de stapel straks uitgaat
        if (cleanupSound != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(cleanupSound, Camera.main.transform.position, soundVolume);

        // Animatie
        if (image != null && cleanupFrames != null)
        {
            foreach (var frame in cleanupFrames)
            {
                image.sprite = frame;
                yield return new WaitForSeconds(frameDuration);
            }
        }

        GiveRewards();

        Cleaned = true;
        NotificationManager.Instance.ShowNotification(notificationText);
        Trophies.CheckBuyEverything();

        gameObject.SetActive(false);
    }

    private void GiveRewards()
    {
        // Coins uit vuilnis tellen niet als 'verdiend' voor de dagwinst
        if (rewardCoins > 0) CoinManager.Instance.AddCoins(rewardCoins, false);
        if (rewardAnimalFood > 0) CoinManager.Instance.AddAnimalFood(rewardAnimalFood);
        if (rewardAnimalFood2 > 0) CoinManager.Instance.AddAnimalFood2(rewardAnimalFood2);
        if (rewardWheat > 0) CoinManager.Instance.AddWheat(rewardWheat);
        if (rewardCorn > 0) CoinManager.Instance.AddCorn(rewardCorn);

        if (rewardSeeds != null)
        {
            foreach (var seed in rewardSeeds)
            {
                if (seed.amount > 0)
                    SeedManager.Instance.AddSeeds(seed.seedType.ToString(), seed.amount);
            }
        }

        // Zwevende popups voor wat er een prefab voor is
        var popups = new List<(HarvestPopup prefab, int amount)>();
        if (rewardCoins > 0 && coinPopupPrefab != null) popups.Add((coinPopupPrefab, rewardCoins));
        if (rewardWheat > 0 && wheatPopupPrefab != null) popups.Add((wheatPopupPrefab, rewardWheat));
        if (rewardCorn > 0 && cornPopupPrefab != null) popups.Add((cornPopupPrefab, rewardCorn));

        for (int i = 0; i < popups.Count; i++)
        {
            float offset = (i - (popups.Count - 1) / 2f) * popupSpacing;
            SpawnPopup(popups[i].prefab, popups[i].amount, offset);
        }
    }

    private void SpawnPopup(HarvestPopup prefab, int amount, float xOffset)
    {
        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;

        // Op het canvas zelf, niet op de stapel, want die gaat zo uit
        HarvestPopup popup = Instantiate(prefab, canvas.transform);

        // Afstanden in canvas-eenheden omrekenen naar wereld-eenheden,
        // zodat het werkt op elk soort canvas (Overlay of Camera)
        float scale = canvas.transform.lossyScale.y;
        popup.riseDistance *= scale;

        Vector3 pos = transform.position + new Vector3(xOffset * scale, 0f, 0f);
        popup.Show(amount, pos);
    }
}
