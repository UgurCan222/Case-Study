using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 Bu script oyunun karakterin yürüyebildiði karelerin manageri. JSON dosyasýndaki verileri okuyup bunlarý sýrayla sahneye diziyor.
 Karakterin ilerleyeceði yolu oluþturup her bir karenin bilgisini (meyve mi, boþ mu vs.) tutuyor.
 Karakter hareket ederken veya bir karede durduðunda, görsel efektleri, sesleri ve envanter sistemini tetikleyen ana merkez burasý.
 */

public class MapManager : MonoBehaviour
{
    // Singleton yapýsý her yerden MapManager.Instance diyerek buna ulaþabilelim diye.
    public static MapManager Instance;

    [Header("Map Settings")]
    [SerializeField] private TextAsset mapJsonFile;     // Harita verilerini tutan JSON dosyamýz
    [SerializeField] private GameObject tilePrefab;    // Sahneye dizilecek her bir kare objesi
    [SerializeField] private float tileSpacing = 1.2f;  // Karelerin arasýndaki mesafe (Z ekseninde)

    [Header("Hierarchy")]
    [SerializeField] private Transform mapParent;       // Oluþan kareler Hierarchy'de kalabalýk yapmasýn, bunun altýna girsin

    [Header("Effect")]
    [SerializeField] private ParticleSystem collectionParticlePrefab; // Meyve alýnca veya geçerken patlayacak efekt

    // Dýþarýdan tile verilerine ve transformlarýna eriþmek gerekirse diye bunlarý tutuyoruz
    public TileData[] GeneratedTiles { get; private set; }
    private List<Transform> tileTransforms = new List<Transform>();

    private void Awake()
    {
        // Singleton'ý initialize ediyoruz
        Instance = this;
    }

    private void Start()
    {
        // Oyun baþlar baþlamaz haritayý JSONdan okuyup dizelim
        GenerateMapFromJSON();
    }

    private void GenerateMapFromJSON()
    {
        if (mapJsonFile == null)
        {
            Debug.LogError("JSON missing!");
            return;
        }

        MapData mapData = JsonUtility.FromJson<MapData>(mapJsonFile.text);

        // tiles yerine node_list
        GeneratedTiles = mapData.node_list;

        for (int i = 0; i < GeneratedTiles.Length; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 0, i * tileSpacing);
            GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, mapParent);

            // tileIndex yerine step
            newTile.name = $"Tile_{GeneratedTiles[i].step}";

            tileTransforms.Add(newTile.transform);

            Tile tileComponent = newTile.GetComponent<Tile>();
            if (tileComponent != null)
            {
                tileComponent.Initialize(GeneratedTiles[i]);
            }
        }
    }

    // Karakterin hareket mantýðý tile listesine ihtiyaç duyarsa diye bu getter'ý kullanýyoruz
    public List<Transform> GetTiles()
    {
        return tileTransforms;
    }

    // Karakter bir karenin üzerinden basýp geçerkenki çalýþan efekt
    public void TriggerPassingEffect(int index)
    {
        Transform targetTileTransform = tileTransforms[index];
        Tile targetTileScript = targetTileTransform.GetComponent<Tile>();

        // Hafifçe glow olsun customtweendeki glow mantýðý
        AnimateTileGlow(targetTileScript);

        // Geçiþ esnasýnda sihir gibi partikül patlatalým
        if (collectionParticlePrefab != null)
        {
            Vector3 spawnPos = targetTileTransform.position + Vector3.up * 1.5f;
            ParticleSystem fx = Instantiate(collectionParticlePrefab, spawnPos, Quaternion.identity);
            // Efekt bitince objeyi yok etmeyi unutmayalým, hafýza þiþmesin
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
        }
    }

    // Karakter zar sonucu durduðu son noktaya ulaþtýðýnda çalýþan asýl fonksiyon
    public void CollectItemOnTile(int index)
    {
        TileData data = GeneratedTiles[index];
        Transform targetTileTransform = tileTransforms[index];
        Tile targetTileScript = targetTileTransform.GetComponent<Tile>();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFinalTileSound();
        }

        StartCoroutine(AnimateTileFlash(targetTileScript, 5));

        if (collectionParticlePrefab != null)
        {
            Vector3 spawnPos = targetTileTransform.position + Vector3.up * 1.5f;
            ParticleSystem fx = Instantiate(collectionParticlePrefab, spawnPos, Quaternion.identity);
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
        }

        if (!data.is_empty && data.amount > 0)
        {
            if (InventoryManager.Instance != null)
            {
                // toplanýlan meyve
                InventoryManager.Instance.AddItem((ItemType)data.fruit, data.amount);
            }
        }

        if (DiceManager.Instance != null)
        {
            DiceManager.Instance.ShowTileResult(data.is_empty, data.amount, (ItemType)data.fruit);
        }
    }

    // parlama animasyonu
    // ColorTo ile peþ peþe (zincirleme) animasyon çaðýrýyor.
    private void AnimateTileGlow(Tile tile)
    {
        if (tile == null || tile.TileMeshRenderer == null) return;

        Material mat = tile.TileMeshRenderer.material;
        mat.EnableKeyword("_EMISSION");
        Color originalEmissionColor = mat.GetColor("_EmissionColor");
        Color targetGlowColor = Color.white * 3.5f; // Parlama þiddeti 3.5 kat olsun

        // Glow-in
        CustomTween.Instance.ColorTo(originalEmissionColor, targetGlowColor, 0.1f,
            (color) =>
            {
                // Renk güncellendikçe materyale bas
                if (mat != null) mat.SetColor("_EmissionColor", color);
            },
            () =>
            {
                // Glow-out
                CustomTween.Instance.ColorTo(targetGlowColor, originalEmissionColor, 0.15f,
                    (color) =>
                    {
                        if (mat != null) mat.SetColor("_EmissionColor", color);
                    },
                    () =>
                    {
                        // En son orijinal rengi
                        if (mat != null) mat.SetColor("_EmissionColor", originalEmissionColor);
                    });
            });
    }

    // Hýzlý hýzlý yanýp sönme efekti (sadece on/off mantýðýyla çalýþtýðý için coroutine kullandým lerp yok)
    private IEnumerator AnimateTileFlash(Tile tile, int flashCount)
    {
        if (tile == null || tile.TileMeshRenderer == null) yield break;

        Material mat = tile.TileMeshRenderer.material;
        mat.EnableKeyword("_EMISSION");
        Color originalEmissionColor = mat.GetColor("_EmissionColor");

        Color targetGlowColor = Color.white * 4f; // Flaþ daha sert olsun
        float flashDuration = 0.08f; // Yanma ve sönme hýzý

        for (int i = 0; i < flashCount; i++)
        {
            // Yak
            mat.SetColor("_EmissionColor", targetGlowColor);
            yield return new WaitForSeconds(flashDuration);

            // Söndür
            mat.SetColor("_EmissionColor", originalEmissionColor);
            yield return new WaitForSeconds(flashDuration);
        }

        mat.SetColor("_EmissionColor", originalEmissionColor);
    }
}