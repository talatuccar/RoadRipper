using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Oyuncunun transform'u
    public Vector3 offset; // Kamera ile oyuncu arasýndaki mesafe
    public float smoothSpeed = 0.125f; // Kameranýn takip hýzý

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset; // Hedef pozisyon
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Yumuþak geçiþ
        transform.position = smoothedPosition; // Kamerayý yeni pozisyona taþý

       
    }
}
