using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Oyuncunun transform'u
    public Vector3 offset; // Kamera ile oyuncu arasındaki mesafe
    public float smoothSpeed = 0.125f; // Kameranın takip hızı

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset; // Hedef pozisyon
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Yumuşak geçiş
        transform.position = smoothedPosition; // Kamerayı yeni pozisyona taşı

        transform.LookAt(player); // Oyuncuya bakma (Opsiyonel)
    }
}

