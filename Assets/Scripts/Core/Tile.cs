using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshPro tileIndexText; // Sýra no (1 2 3 4..)
    [SerializeField] private GameObject rewardContainer;
    [SerializeField] private TextMeshPro rewardAmountText; // Meyve miktarý ve ismi (örnek: 15 Strawberry)

    [Header("Tile Mesh & Materials")]
    [SerializeField] private MeshRenderer tileMeshRenderer;
    [SerializeField] private Material emptyMaterial;
    [SerializeField] private Material appleMaterial;
    [SerializeField] private Material pearMaterial;
    [SerializeField] private Material strawberryMaterial;

    // Iþýma efekti için mapmanagerin eriþimine açtýðýmýz özellik
    public MeshRenderer TileMeshRenderer => tileMeshRenderer;

    public TileData Data { get; private set; }

    public void Initialize(TileData data)
    {
        Data = data;

        if (tileMeshRenderer == null)
        {
            tileMeshRenderer = GetComponent<MeshRenderer>();
            if (tileMeshRenderer == null)
            {
                tileMeshRenderer = GetComponentInChildren<MeshRenderer>();
            }
        }

        // Kare indeksi
        if (tileIndexText != null)
        {
            tileIndexText.text = data.step.ToString();
        }

        // Boþ kare
        if (data.is_empty || data.amount <= 0)
        {
            // Boþlarda Empty yazsýn
            if (rewardContainer != null) rewardContainer.SetActive(true);
            if (rewardAmountText != null) rewardAmountText.text = "Empty";

            if (tileMeshRenderer != null && emptyMaterial != null)
                tileMeshRenderer.material = emptyMaterial;
        }
        // Dolu kare
        else
        {
            if (rewardContainer != null) rewardContainer.SetActive(true);

            // Kazanýlan meyve sayýsý ve meyve ismi (amount ve fruit)
            if (rewardAmountText != null)
            {
                rewardAmountText.text = $"{data.amount} {(ItemType)data.fruit}";
            }

            if (tileMeshRenderer != null)
            {
                switch ((ItemType)data.fruit)
                {
                    case ItemType.Apple:
                        if (appleMaterial != null) tileMeshRenderer.material = appleMaterial;
                        break;
                    case ItemType.Pear:
                        if (pearMaterial != null) tileMeshRenderer.material = pearMaterial;
                        break;
                    case ItemType.Strawberry:
                        if (strawberryMaterial != null) tileMeshRenderer.material = strawberryMaterial;
                        break;
                }
            }
        }

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        CustomTween.Instance.ScaleTo(transform, originalScale, 0.5f);
    }
}