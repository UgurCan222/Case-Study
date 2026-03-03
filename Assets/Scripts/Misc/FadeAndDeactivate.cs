using UnityEngine;
using UnityEngine.UI;

public class FadeAndDeactivate : MonoBehaviour
{
    /*
    Bu script baþtaki unity'nin splash screen'i bitince anýnda oyun rengi gözükmesin yumuþak geçiþ olsun diye splash screendeki
    ayný renk düz bir ui gameobjecte uygulanacak þekilde yazýlmýþtýr. Fade-out efekti uyguluyor.
    Ama ayrý lerp yazmadým tabi ki customtween'e baþvuruyoruz.
    */
    public float fadeDuration = 1f;
    public bool fadeOnStart = false;

    private Image targetImage;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (fadeOnStart)
        {
            TriggerFadeOut();
        }
    }

    public void TriggerFadeOut()
    {
        if (targetImage != null && gameObject.activeInHierarchy)
        {
            // max transparanlýk için hedef alpha deðeri 0, bitiþ eventi olarak ise obje deaktif (destroy kullanmak istemedim boþ obje olduðu için)
            CustomTween.Instance.FadeTo(targetImage, 0f, fadeDuration, () =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}