using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button muteButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    // Sesin þu anki durumu
    private bool isMuted = false;
    private Image muteButtonImage;

    private void Start()
    {
        // Quit Buton
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("inspector>quit buton");
        }

        // Mute Buton
        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
            muteButtonImage = muteButton.GetComponent<Image>();

            // Eðer daha önceden oyuncu sesi kapattýysa
            isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
            ApplyMuteState();
        }
        else
        {
            Debug.LogWarning("inspector>mute buton");
        }
    }
    private void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMuteState();

        // tercihi kaydet
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyMuteState()
    {
        // Tüm sesleri aç kapa. ayrý ayrý listener referansýna gerek yok
        AudioListener.volume = isMuted ? 0f : 1f;

        // Görseli duruma göre deðiþ
        if (muteButtonImage != null)
        {
            muteButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}