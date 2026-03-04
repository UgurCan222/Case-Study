using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Cam Speed")]
    [SerializeField] private float followDuration = 1.0f; // Kamera geriden gelsin diye

    [Header("Z limit")]
    [SerializeField] private float minZ = 1.9f; //Baþlangýçtaki kamera z deðeri
    [SerializeField] private float maxZ = 8.69f; //Sondaki kamera z deðeri

    private float initialZOffset;
    private Coroutine currentCamTween;

    // CustomTween'in her frame çekebileceði dinamik hedefimiz
    private float currentTargetZ;

    private bool isReturningToStart = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
        }

        if (player != null)
        {
            Vector3 startPos = transform.position;
            startPos.z = minZ;
            transform.position = startPos;

            initialZOffset = transform.position.z - player.position.z;
            currentTargetZ = minZ; // Baþlangýç hedefi
        }
    }

    public void MoveCameraToZ(float targetPlayerZ)
    {
        if (player == null || isReturningToStart) return;

        // CustomTween'deki dinamik rutinin okuyacaðý hedefi güncelliyoruz
        currentTargetZ = Mathf.Clamp(targetPlayerZ + initialZOffset, minZ, maxZ);

        // Eðer dinamik takip coroutine'i çalýþmýyorsa baþlatýyoruz
        // Böylece her frame durdur-baþlat yapmadan var olan tek bir tween hedefe akar.
        if (currentCamTween == null)
        {
            currentCamTween = CustomTween.Instance.FollowDynamicZ(transform, () => currentTargetZ, followDuration);
        }
    }

    public void ReturnToStart()
    {
        isReturningToStart = true;

        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, minZ);

        if (currentCamTween != null)
        {
            CustomTween.Instance.StopCoroutine(currentCamTween);
        }

        // Return iþlemi sabit bir noktaya olduðu için normal MoveTo tweenimizi kullanýyoruz
        currentCamTween = CustomTween.Instance.MoveTo(transform, startPos, followDuration);
    }

    public void ResumeFollowing()
    {
        isReturningToStart = false;

        if (currentCamTween != null)
        {
            CustomTween.Instance.StopCoroutine(currentCamTween);
            currentCamTween = null; // Null yapýyoruz ki MoveCameraToZ çaðrýldýðýnda dinamik takip baþtan baþlasýn
        }
    }
}