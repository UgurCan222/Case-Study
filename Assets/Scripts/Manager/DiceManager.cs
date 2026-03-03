using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    // Diðer scriptlerin ulaþmasý için singleton
    public static DiceManager Instance;

    [Header("Dice count")]
    public int baslangicZarSayisi = 2;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI diceCountText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button rollButton;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Transform inputContainer;

    [SerializeField] private GameObject diceRowPrefab;

    [Header("Sub")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Dice")]
    [SerializeField] private Dice3D dicePrefab;
    [SerializeField] private Transform throwStartPoint;
    [SerializeField] private Transform throwEndPoint;

    private int currentDiceCount;
    private List<TMP_InputField> currentInputs = new List<TMP_InputField>();
    private List<Dice3D> activeDices = new List<Dice3D>();

    // durum ve animasyon kontrolü
    private bool isRolling = false;
    private Coroutine textAnimationCoroutine;
    private Coroutine breathingCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentDiceCount = baslangicZarSayisi;

        // listenerlar
        if (plusButton != null) plusButton.onClick.AddListener(() => ChangeDiceCount(1));
        if (minusButton != null) minusButton.onClick.AddListener(() => ChangeDiceCount(-1));
        if (rollButton != null) rollButton.onClick.AddListener(RollDices);

        UpdateDiceCountText();
        GenerateInputFields(currentDiceCount);

        // bekleme ve animasyon
        if (statusText != null)
        {
            // --Tercihen bi bold yaptým fikir deðiþtirirsem kaldýrýrým, þimdilik dursun
            statusText.fontStyle = FontStyles.Bold;
            ShowWaitingState();
        }
    }

    private void ChangeDiceCount(int change)
    {
        if (isRolling) return; // Zarlar atýlýrken ui'da deðiþiklik olmasýn

        currentDiceCount += change;
        if (currentDiceCount < 1) currentDiceCount = 1;
        if (currentDiceCount > 20) currentDiceCount = 20;

        UpdateDiceCountText();
        GenerateInputFields(currentDiceCount);
    }

    private void UpdateDiceCountText()
    {
        if (diceCountText != null)
        {
            diceCountText.text = currentDiceCount + " Dice";
        }
    }

    private void GenerateInputFields(int count)
    {
        // Fazla input yerlerini sil
        while (currentInputs.Count > count)
        {
            int lastIndex = currentInputs.Count - 1;
            TMP_InputField fieldToRemove = currentInputs[lastIndex];

            // TMP_Input objesinin bir üst atasý DiceRow objesi onu bulup sil
            GameObject rowToRemove = fieldToRemove.transform.parent.gameObject;
            rowToRemove.transform.SetParent(null); // Aileden çýkardýk
            Destroy(rowToRemove);

            currentInputs.RemoveAt(lastIndex);
        }

        // Artýrdýysak yenisini ekle
        while (currentInputs.Count < count)
        {
            GameObject rowGO = Instantiate(diceRowPrefab, inputContainer);

            Transform labelTransform = rowGO.transform.Find("Label");
            Transform inputTransform = rowGO.transform.Find("TMP_Input");

            if (labelTransform != null && inputTransform != null)
            {
                TextMeshProUGUI labelTxt = labelTransform.GetComponent<TextMeshProUGUI>();
                TMP_InputField inputField = inputTransform.GetComponent<TMP_InputField>();

                int newIndex = currentInputs.Count;
                labelTxt.text = "Dice " + (newIndex + 1) + ":";

                // Eðer listede önceden eklenmiþ bir zar varsa deðerini ondan kopyala, tekrar tekrar 6 ile uðraþma
                if (newIndex > 0)
                {
                    inputField.text = currentInputs[newIndex - 1].text;
                }
                else
                {
                    inputField.text = "6"; // Ama yoksa 6 ver geç
                }

                currentInputs.Add(inputField);
            }
            else
            {
                Debug.LogWarning("DiceRow prefabýnýn içinde Label veya TMP_Input bulunamadý");
                break; // Hata durumunda sonsuz döngüyü engelle
            }
        }

        // Layout'un kendini anýnda güncellemesi için
        Canvas.ForceUpdateCanvases();
    }

    #region STATUS TEXT ANIMATIONS

    private void ShowWaitingState()
    {
        if (statusText == null) return;

        if (breathingCoroutine != null) StopCoroutine(breathingCoroutine);
        breathingCoroutine = StartCoroutine(BreathingRoutine());
    }

    private IEnumerator BreathingRoutine()
    {
        statusText.text = "Waiting for you to roll...";
        // Normalde yazý beyaz olsun
        statusText.color = Color.white;

        float time = 0;
        Vector3 baseScale = Vector3.one;

        // Customtween sonsuz döngü desteklemediði için bu animasyon özel olarak coroutine kalsýn
        while (true)
        {
            time += Time.deltaTime * 2.5f;
            float scale = 1f + Mathf.Sin(time) * 0.05f;
            statusText.transform.localScale = baseScale * scale;
            yield return null;
        }
    }

    private IEnumerator TypewriterEffect(string text)
    {
        if (statusText == null) yield break;

        statusText.text = "";
        // Yazý yazma efekti anýnda da renk beyaz
        statusText.color = Color.white;
        statusText.transform.localScale = Vector3.one;

        foreach (char c in text)
        {
            statusText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator FadeOutEffect()
    {
        if (statusText == null) yield break;

        Color startColor = statusText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        bool isFadeComplete = false;

        // ColorTo'yu cagýrýyoruz
        CustomTween.Instance.ColorTo(startColor, endColor, 0.5f,
            (color) =>
            {
                if (statusText != null) statusText.color = color;
            },
            () =>
            {
                if (statusText != null) statusText.text = "";
                isFadeComplete = true;
            });

        yield return new WaitUntil(() => isFadeComplete);
    }

    // Sonuc gösterim
    public void ShowTileResult(bool isEmpty, int amount, ItemType itemType)
    {
        if (textAnimationCoroutine != null) StopCoroutine(textAnimationCoroutine);
        if (breathingCoroutine != null) StopCoroutine(breathingCoroutine);

        StartCoroutine(RewardAnimationRoutine(isEmpty, amount, itemType));
    }

    private IEnumerator RewardAnimationRoutine(bool isEmpty, int amount, ItemType itemType)
    {
        if (statusText != null)
        {
            statusText.transform.localScale = Vector3.one;

            if (isEmpty || amount <= 0)
            {
                // Boþ karenin rengi pastel mavi
                statusText.color = new Color(0.6f, 0.8f, 1f, 1f);
                statusText.text = "Arrived on an Empty Tile...";
            }
            else
            {
                Color fruitColor = Color.white;
                string fruitName = "";

                // Meyveler için yine pastel tonlarý
                switch (itemType)
                {
                    case ItemType.Apple:
                        fruitColor = new Color(1f, 0.7f, 0.4f, 1f); // Pastel turuncu
                        fruitName = "Apple";
                        break;
                    case ItemType.Pear:
                        fruitColor = new Color(0.6f, 0.9f, 0.6f, 1f); // Pastel yeþil
                        fruitName = "Pear";
                        break;
                    case ItemType.Strawberry:
                        fruitColor = new Color(1f, 0.5f, 0.5f, 1f); // Pastel kýrmýzý
                        fruitName = "Strawberry";
                        break;
                }

                statusText.color = fruitColor;
                statusText.text = $"<size=120%>+{amount}</size> {fruitName} Collected!";

                // Büyüme kücülme animasyonu
                bool isPopComplete = false;
                CustomTween.Instance.ScaleTo(statusText.transform, Vector3.one * 1.3f, 0.1f, () =>
                {
                    CustomTween.Instance.ScaleTo(statusText.transform, Vector3.one, 0.1f, () =>
                    {
                        isPopComplete = true;
                    });
                });

                yield return new WaitUntil(() => isPopComplete);
            }

            // Oyuncu okusun diye biraz bekle
            yield return new WaitForSeconds(1.5f);

            // Þýk bir þekilde silin
            yield return StartCoroutine(FadeOutEffect());
        }

        // Tüm süreç bittikten sonra ekraný bekleme durumuna çevir 
        ShowWaitingState();

        isRolling = false;
        if (rollButton != null) rollButton.interactable = true;
    }

    #endregion

    #region 3D DICE THROW & MOVEMENT LOGIC

    private void RollDices()
    {
        if (isRolling) return;

        isRolling = true;
        if (rollButton != null) rollButton.interactable = false;

        // Swoosh þeklinde zar atma sesi
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDiceThrow();
        }

        int targetDiceCount = currentInputs.Count;
        int totalSum = 0;
        List<string> diceStrings = new List<string>();

        while (activeDices.Count > targetDiceCount)
        {
            Dice3D excessDice = activeDices[activeDices.Count - 1];
            excessDice.FlyAway(0f);
            activeDices.RemoveAt(activeDices.Count - 1);
        }

        int maxColumns = 3;
        int columns = Mathf.Min(targetDiceCount, maxColumns);
        int rows = Mathf.CeilToInt((float)targetDiceCount / columns);

        // Buradaki Mathf.Lerp bir animasyon interpolasyonu deðildir. Zar boyutunun anlýk matematiksel oranýný hesaplar
        float scaleVal = targetDiceCount <= 5 ? 0.7f : Mathf.Lerp(0.7f, 0.35f, (targetDiceCount - 5) / 15f);
        Vector3 targetScale = Vector3.one * scaleVal;

        float baseSpacing = 0.85f;
        float spacingX = baseSpacing * scaleVal;
        float spacingZ = baseSpacing * scaleVal;

        for (int i = 0; i < targetDiceCount; i++)
        {
            int row = i / columns;
            int col = i % columns;

            int itemsInThisRow = (row == rows - 1) ? (targetDiceCount - (row * columns)) : columns;

            // Temel Grid Pozisyonu
            float offsetX = (col - (itemsInThisRow - 1) / 2f) * spacingX;
            float offsetY = 0f;
            float offsetZ = -row * spacingZ;

            float randomJitterX = Random.Range(-0.15f, 0.15f) * scaleVal;
            float randomJitterZ = Random.Range(-0.15f, 0.15f) * scaleVal;

            Vector3 gridOffset = new Vector3(offsetX + randomJitterX, offsetY, offsetZ + randomJitterZ);
            Vector3 landingPos = throwEndPoint.position + gridOffset;

            int.TryParse(currentInputs[i].text, out int diceValue);

            diceValue = Mathf.Max(1, diceValue);
            totalSum += diceValue;
            diceStrings.Add(diceValue.ToString());

            if (i < activeDices.Count)
            {
                activeDices[i].RollInPlace(diceValue, landingPos, i * 0.05f);
                if (CustomTween.Instance != null)
                    CustomTween.Instance.ScaleTo(activeDices[i].transform, targetScale, 0.2f);
            }
            else
            {
                Dice3D newDice = Instantiate(dicePrefab, throwStartPoint.position, Quaternion.identity);
                newDice.transform.localScale = targetScale;
                activeDices.Add(newDice);
                newDice.RollTo(diceValue, landingPos, i * 0.05f);
            }
        }

        Debug.Log("zarlar atýldý toplam: totalSum");
        //Kullanýcýya göster toplam kaç sayýlýk zar atýlmýþ geriye kaç adým kalmýþ vs.
        string calcString = string.Join(" + ", diceStrings) + " = " + totalSum + " Steps!";

        float maxAnimationTime = ((targetDiceCount - 1) * 0.05f) + 0.8f;
        StartCoroutine(WaitAndMove(totalSum, maxAnimationTime, calcString));
    }

    private IEnumerator WaitAndMove(int totalSteps, float waitTime, string calcString)
    {
        if (breathingCoroutine != null) StopCoroutine(breathingCoroutine);
        if (textAnimationCoroutine != null) StopCoroutine(textAnimationCoroutine);

        textAnimationCoroutine = StartCoroutine(TypewriterEffect(calcString));

        yield return new WaitForSeconds(waitTime + 0.3f);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.Move(totalSteps);
        }
        else
        {
            Debug.LogError("PlayerController not found in the scene!");
        }

        int stepsLeft = totalSteps;
        while (stepsLeft > 0)
        {
            if (statusText != null)
            {
                // Yürürkenki "Moving..." yazýsýnýn her zaman beyaz olduðundan emin oluyoruz
                statusText.color = Color.white;
                statusText.text = $"Moving... {stepsLeft} steps left";
            }

            yield return new WaitForSeconds(0.4f);
            stepsLeft--;
        }

        // Oyuncu yürümeyi bitirdiði an buradaki iþlemimiz biter.
        // Daha sonra MapManager.CollectItemOnTile fonksiyonu otomatik çalýþarak 
        // bizim yukarýda yazdýðýmýz "ShowTileResult" metodumuzu tetikleyip ödül animasyonunu baþlatacak.
    }

    #endregion
}