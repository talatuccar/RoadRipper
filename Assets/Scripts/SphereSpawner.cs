using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{

    [SerializeField] private GameObject spherePrefab;  // Referans verdiðin prefab
    [SerializeField] private int poolSize = 10;        // Havuzdaki küre sayýsý
    [SerializeField] private float activeTime = 5f;    // Kürelerin sahnede kalma süresi
    [SerializeField] private float sphereSpeed = 5f;    // Kürelerin hareket hýzý

    private Queue<GameObject> spherePool;              // Havuz yapýsý
    private float[] lanePositions = { -2.5f, 0f, 2.5f };    // Sol, orta, sað pozisyonlarý

    void Start()
    {
        // Havuzu oluþtur
        spherePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            sphere.SetActive(false); // Baþlangýçta aktif olmasýn
            spherePool.Enqueue(sphere); // Havuzda sýraya al
        }

        InvokeRepeating("SpawnSphere", 2f, 2f); // Küreleri belli aralýklarla üret
    }

    void SpawnSphere()
    {
        if (spherePool.Count > 0)
        {
            GameObject sphere = spherePool.Dequeue(); // Havuzdan bir küre al

            // Rastgele þerit seçimi
            int randomLaneIndex = Random.Range(0, lanePositions.Length);
            int zPos = Random.Range(30, 150);
            sphere.transform.position = new Vector3(lanePositions[randomLaneIndex], 0.6f, zPos);
            sphere.SetActive(true); // Küreyi sahnede aktif et

            // Kürenin hareket etmesini saðla
            StartCoroutine(MoveAndDeactivateSphere(sphere, activeTime));
        }
    }

    IEnumerator MoveAndDeactivateSphere(GameObject sphere, float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            // Küreyi ileri doðru hareket ettir
            sphere.transform.Translate(-Vector3.forward * sphereSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Küre sahnede belli süre kaldýktan sonra inaktif yap ve havuza geri koy
        sphere.SetActive(false);
        spherePool.Enqueue(sphere);
    }
}















