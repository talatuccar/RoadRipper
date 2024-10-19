using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Oyuncunun transform'u
    public Vector3 offset; // Kamera ile oyuncu aras�ndaki mesafe
    public float smoothSpeed = 0.125f; // Kameran�n takip h�z�

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset; // Hedef pozisyon
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Yumu�ak ge�i�
        transform.position = smoothedPosition; // Kameray� yeni pozisyona ta��

       
    }
}
