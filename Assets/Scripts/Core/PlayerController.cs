using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
Burada karakter zar atýldýktan sonra kaç adým gitmesi gerekiyorsa, o kadar kareyi tek tek zýplayarak geziyor.
Hareket iþlemi tamamen CustomTween üzerinden yönetiliyor, böylece kod tekrarýndan kaçýnýlýyor.
Her kareye bastýðýnda adým sesi çýkartýyor ve geçtiði karelerin parlamasýný tetikliyor.
Yolculuk bittiðinde ise durduðu son karedeki meyveyi toplamasýný söylüyor.
 */

public class PlayerController : MonoBehaviour
{
    // Singleton yapýsý ile karakter tek olduðu için her yerden PlayerController.Instance ile eriþelim.
    public static PlayerController Instance;

    [Header("Movement Settings")]
    [SerializeField] float moveDuration = 0.4f; // Bir kareden diðerine zýplama hýzý (sn)
    [SerializeField] float jumpHeight = 0.5f;   // Zýplama yüksekliði

    [SerializeField] float yOffset = 0.748f;    // Karakterin karelerin tam üstünde durmasý için gereken yükseklik ayarý

    private int currentTileIndex = 0; // Þu an hangi karedeyiz
    private bool isMoving = false;    // Karakter hareket halinde mi

    private void Awake()
    {
        // Singleton ý kuruyoruz
        Instance = this;
    }

    // Zar atýldýðýnda çaðrýlacak ana fonksiyon
    public void Move(int totalSteps)
    {
        // Eðer zaten yürüyorsak yeni bir hareket komutunu kabul etme
        if (isMoving) return;

        // Asýl hareket mantýðýný coroutine içinde baþlatýyoruz
        StartCoroutine(MoveRoutine(totalSteps));
    }

    private IEnumerator MoveRoutine(int totalSteps)
    {
        isMoving = true;

        // MapManagerdan güncel kare listesini alýyoruz
        List<Transform> tiles = MapManager.Instance.GetTiles();

        // Atýlan zar miktarý kadar döngüye girip adým atýyoruz
        for (int i = 0; i < totalSteps; i++)
        {
            currentTileIndex++;

            // Eðer listenin sonuna geldiysek baþa dön
            if (currentTileIndex >= tiles.Count)
            {
                currentTileIndex = 0;
            }

            Vector3 endPos = tiles[currentTileIndex].position + new Vector3(0, yOffset, 0);

            // Zýplama iþleminin bitip bitmediðini takip etmek için bir bayrak
            bool isStepComplete = false;

            // Hareketi CustomTween'e devrediyoruz ve bittiðinde true yapýyoruz
            CustomTween.Instance.JumpTo(transform, endPos, jumpHeight, moveDuration, () =>
            {
                isStepComplete = true;
            });

            // CustomTween animasyonu bitirene kadar bekletiyoruz
            yield return new WaitUntil(() => isStepComplete);

            // Adým tamamlandý

            // Her yere iniþ yaptýðýnda adým sesi çalsýn
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayStepSound();
            }

            // Eðer bu durduðumuz son kare deðilse
            if (i < totalSteps - 1)
            {
                // MapManager'a parlat diyoruz
                MapManager.Instance.TriggerPassingEffect(currentTileIndex);
            }
        }

        // Karakter son duraðýna ulaþtý
        // MapManager'a haber ver, ödül varsa versin, final sesini çalsýn.
        MapManager.Instance.CollectItemOnTile(currentTileIndex);

        // Hareketimiz bittiðine göre yeni zar atýþýna hazýr
        isMoving = false;
    }
}