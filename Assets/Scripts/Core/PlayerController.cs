using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement Settings")]
    [SerializeField] float moveDuration = 0.4f; // Bir kareden diðerine zýplama hýzý (sn)
    [SerializeField] float jumpHeight = 0.5f;   // Zýplama yüksekliði

    [SerializeField] float yOffset = 0.748f;    // Karakterin karelerin tam üstünde durmasý için gereken Y deðeri

    private int currentTileIndex = 0; // Þu an hangi karedeyiz
    private bool isMoving = false;    // Karakter hareket halinde mi

    private void Awake()
    {
        Instance = this;
    }

    public void Move(int totalSteps)
    {
        if (isMoving) return;

        // Hareket baþlýyor, kameraya takibe hazýrlanmasýný söyle
        if (CameraController.Instance != null)
        {
            CameraController.Instance.ResumeFollowing();
        }

        StartCoroutine(MoveRoutine(totalSteps));
    }

    private IEnumerator MoveRoutine(int totalSteps)
    {
        isMoving = true;

        List<Transform> tiles = MapManager.Instance.GetTiles();

        for (int i = 0; i < totalSteps; i++)
        {
            currentTileIndex++;

            if (currentTileIndex >= tiles.Count)
            {
                currentTileIndex = 0;
            }

            Vector3 endPos = tiles[currentTileIndex].position + new Vector3(0, yOffset, 0);

            // Karakter bir sonraki kareye zýplarken kameraya da hedeflenen Z pozisyonuna gitmesini söylüyoruz
            if (CameraController.Instance != null)
            {
                CameraController.Instance.MoveCameraToZ(endPos.z);
            }

            bool isStepComplete = false;

            CustomTween.Instance.JumpTo(transform, endPos, jumpHeight, moveDuration, () =>
            {
                isStepComplete = true;
            });

            yield return new WaitUntil(() => isStepComplete);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayStepSound();
            }

            if (i < totalSteps - 1)
            {
                MapManager.Instance.TriggerPassingEffect(currentTileIndex);
            }
        }

        MapManager.Instance.CollectItemOnTile(currentTileIndex);

        // Adýmlar tamamen bittiðinde kamerayý tekrar 1.9'a gönder
        if (CameraController.Instance != null)
        {
            CameraController.Instance.ReturnToStart();
        }

        isMoving = false;
    }
}