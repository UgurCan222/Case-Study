using UnityEngine;
using System.Collections;

public class Dice3D : MonoBehaviour
{
    //Zar modelini asset store'dan indirdim, 6 yüzlü klasik zar modeli. Þimdi hangi yüzü yukarýdaysa hangi deðere denk geliyor diye liste yapýyoruz.
    [SerializeField] private Vector3[] faceRotations = new Vector3[6];

    public void RollTo(int targetNumber, Vector3 endPosition, float delay)
    {
        StartCoroutine(RollRoutine(targetNumber, endPosition, delay, 3f));
    }

    public void RollInPlace(int targetNumber, Vector3 endPosition, float delay)
    {
        StartCoroutine(RollRoutine(targetNumber, endPosition, delay, 1.5f));
    }

    public void FlyAway(float delay)
    {
        StartCoroutine(FlyAwayRoutine(delay));
    }

    private IEnumerator RollRoutine(int target, Vector3 end, float delay, float jumpHeight)
    {
        // Önce belirtilen süre kadar hareketsiz bekle
        yield return new WaitForSeconds(delay);

        // Hedef yüzeyin açýsýný hesaplayip y ekseninde rastgele bir yön ekleyelim ki çok düzgün dizilmesinler.
        Vector3 targetRotation = faceRotations[(target - 1) % 6];
        targetRotation.y += Random.Range(0, 360f);

        float duration = 0.8f;

        // Hedef rotasyona ek olarak 3 ile 5 tam tur arasý (1080 - 1800 derece) ekstra dönüþ ekliyoruz
        Vector3 extraSpins = new Vector3(
            Random.Range(3, 6) * 360f,
            Random.Range(3, 6) * 360f,
            Random.Range(3, 6) * 360f
        );

        Vector3 startRotation = targetRotation + extraSpins;

        // Animasyonlarýn bitiþini takip
        bool isJumpComplete = false;
        bool isRotateComplete = false;

        // Zýplayarak hedefe git
        CustomTween.Instance.JumpTo(transform, end, jumpHeight, duration, () => isJumpComplete = true);

        // Hesaplanmýþ çoklu takla açýsýndan (startRotation) olmasý gereken yüzeye (targetRotation) dön
        CustomTween.Instance.RotateFromTo(transform, startRotation, targetRotation, duration, () => isRotateComplete = true);

        // Her iki customtween iþleminin de ayný anda bitmesini bekle
        yield return new WaitUntil(() => isJumpComplete && isRotateComplete);

        // Yere çarpma efekti ve sesi
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDiceImpact();

        // Çarptýðýnda scale animasyonu
        Vector3 originalScale = transform.localScale;
        CustomTween.Instance.ScaleTo(transform, originalScale * 1.1f, 0.1f, () => {
            CustomTween.Instance.ScaleTo(transform, originalScale, 0.1f);
        });
    }

    private IEnumerator FlyAwayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 0.5 saniye içerisinde mevcut konumundan 10f yukarýya uç
        Vector3 targetPosition = transform.position + new Vector3(0, 10f, 0);
        bool isFlyComplete = false;

        CustomTween.Instance.MoveTo(transform, targetPosition, 0.5f, () => isFlyComplete = true);

        // Uçma iþlemi bitene kadar bekle
        yield return new WaitUntil(() => isFlyComplete);

        // Ekranda kaybolduðunda objeyi yok edelim. Þimdilik object pool koymadým oyunda az obje var diye.
        Destroy(gameObject);
    }
}