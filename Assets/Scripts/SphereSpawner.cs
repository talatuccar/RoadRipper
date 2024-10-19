using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{

    [SerializeField] private GameObject spherePrefab;  // Referans verdi�in prefab
    [SerializeField] private int poolSize = 10;        // Havuzdaki k�re say�s�
    [SerializeField] private float activeTime = 5f;    // K�relerin sahnede kalma s�resi
    [SerializeField] private float sphereSpeed = 5f;    // K�relerin hareket h�z�

    private Queue<GameObject> spherePool;              // Havuz yap�s�
    private float[] lanePositions = { -2.5f, 0f, 2.5f };    // Sol, orta, sa� pozisyonlar�

    void Start()
    {
        // Havuzu olu�tur
        spherePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            sphere.SetActive(false); // Ba�lang��ta aktif olmas�n
            spherePool.Enqueue(sphere); // Havuzda s�raya al
        }

        InvokeRepeating("SpawnSphere", 2f, 2f); // K�releri belli aral�klarla �ret
    }

    void SpawnSphere()
    {
        if (spherePool.Count > 0)
        {
            GameObject sphere = spherePool.Dequeue(); // Havuzdan bir k�re al

            // Rastgele �erit se�imi
            int randomLaneIndex = Random.Range(0, lanePositions.Length);
            int zPos = Random.Range(30, 150);
            sphere.transform.position = new Vector3(lanePositions[randomLaneIndex], 0.6f, zPos);
            sphere.SetActive(true); // K�reyi sahnede aktif et

            // K�renin hareket etmesini sa�la
            StartCoroutine(MoveAndDeactivateSphere(sphere, activeTime));
        }
    }

    IEnumerator MoveAndDeactivateSphere(GameObject sphere, float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            // K�reyi ileri do�ru hareket ettir
            sphere.transform.Translate(-Vector3.forward * sphereSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // K�re sahnede belli s�re kald�ktan sonra inaktif yap ve havuza geri koy
        sphere.SetActive(false);
        spherePool.Enqueue(sphere);
    }
}















