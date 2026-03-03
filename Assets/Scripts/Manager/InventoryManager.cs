using UnityEngine;
using TMPro;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    /*
     Oyun kapanýp açýldýðýnda kalýnan yerden devam etmek için envanter verilerini (elma,armut ve çilek) PlayerPrefs ile hafýzada tutuyorum þimdilik.
    */
    public static InventoryManager Instance;

    [Header("UI")]

    [SerializeField] private TextMeshProUGUI appleText;
    [SerializeField] private TextMeshProUGUI pearText;
    [SerializeField] private TextMeshProUGUI strawberryText;

    // Gerçek envanter verileri
    private int appleCount = 0;
    private int pearCount = 0;
    private int strawberryCount = 0;

    // Ekranda o gözüken sayýlar
    private int displayAppleCount = 0;
    private int displayPearCount = 0;
    private int displayStrawberryCount = 0;

    // Devam eden sayma animasyonlarýný takip etmek için
    private Coroutine appleRoutine;
    private Coroutine pearRoutine;
    private Coroutine strawberryRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Oyun baþladýðýnda eski verileri yükle ve ekrana yazdýr
        LoadInventory();
        UpdateUI();
    }

    public void AddItem(ItemType itemType, int amount)
    {
        float countDuration = 0.5f;

        switch (itemType)
        {
            case ItemType.Apple:
                appleCount += amount;
                // Önceki animasyon çalýþýyorsa iptal et
                if (appleRoutine != null) CustomTween.Instance.StopCoroutine(appleRoutine);

                // Ýþi customtween'e ver, güncel sayýyý Action üzerinden al
                appleRoutine = CustomTween.Instance.CountTo(displayAppleCount, appleCount, countDuration, (val) =>
                {
                    displayAppleCount = val;
                    if (appleText != null) appleText.text = val.ToString();
                });
                break;

            case ItemType.Pear:
                pearCount += amount;
                if (pearRoutine != null) CustomTween.Instance.StopCoroutine(pearRoutine);

                pearRoutine = CustomTween.Instance.CountTo(displayPearCount, pearCount, countDuration, (val) =>
                {
                    displayPearCount = val;
                    if (pearText != null) pearText.text = val.ToString();
                });
                break;

            case ItemType.Strawberry:
                strawberryCount += amount;
                if (strawberryRoutine != null) CustomTween.Instance.StopCoroutine(strawberryRoutine);

                strawberryRoutine = CustomTween.Instance.CountTo(displayStrawberryCount, strawberryCount, countDuration, (val) =>
                {
                    displayStrawberryCount = val;
                    if (strawberryText != null) strawberryText.text = val.ToString();
                });
                break;
        }

        // hemen kaydet
        SaveInventory();

        // Toplama anýnda yazýlara ufak bir zýplama (scale büyütme ve geri küçültme) efekti
        AnimateText(itemType);
    }

    private void UpdateUI()
    {
        if (appleText != null) appleText.text = displayAppleCount.ToString();
        if (pearText != null) pearText.text = displayPearCount.ToString();
        if (strawberryText != null) strawberryText.text = displayStrawberryCount.ToString();
    }

    private void SaveInventory()
    {
        // PlayerPrefs ile verileri cihazýn hafýzasýna yazýyoruz
        PlayerPrefs.SetInt("AppleCount", appleCount);
        PlayerPrefs.SetInt("PearCount", pearCount);
        PlayerPrefs.SetInt("StrawberryCount", strawberryCount);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        // Kayýt yoksa varsayýlan olarak 0 getirir
        appleCount = PlayerPrefs.GetInt("AppleCount", 0);
        pearCount = PlayerPrefs.GetInt("PearCount", 0);
        strawberryCount = PlayerPrefs.GetInt("StrawberryCount", 0);

        // Baþlangýçta ekrandaki sayýlar da gerçek sayýlara eþit olmalý
        displayAppleCount = appleCount;
        displayPearCount = pearCount;
        displayStrawberryCount = strawberryCount;
    }

    // Yumuþak modern görsellik için ufak animasyon
    private void AnimateText(ItemType itemType)
    {
        Transform targetTransform = null;

        switch (itemType)
        {
            case ItemType.Apple:
                if (appleText != null) targetTransform = appleText.transform;
                break;
            case ItemType.Pear:
                if (pearText != null) targetTransform = pearText.transform;
                break;
            case ItemType.Strawberry:
                if (strawberryText != null) targetTransform = strawberryText.transform;
                break;
        }

        if (targetTransform != null && CustomTween.Instance != null)
        {
            // Yazý önce 1.5 katýna büyür, sonra yavaþça kendi boyutuna döner
            CustomTween.Instance.ScaleTo(targetTransform, Vector3.one * 1.5f, 0.1f, () => {
                CustomTween.Instance.ScaleTo(targetTransform, Vector3.one, 0.1f);
            });
        }
    }
}