//using UnityEngine;

//public class PowerUpSpawner : MonoBehaviour
//{
//    [Header("All PowerUp Prefabs (Sprint + Slow)")]
//    [SerializeField] private GameObject[] powerUpPrefabs; // private + SerializeField juga bisa

//    private void Start()
//    {
//        Spawn();
//    }

//    public void Spawn()
//    {
//        if (powerUpPrefabs.Length == 0) return;

//        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
//        Instantiate(prefab, transform.position, Quaternion.identity);
//    }
//}

using UnityEngine;
using System.Collections; // Wajib untuk Coroutine

public class PowerUpSpawner : MonoBehaviour
{
    [Header("All PowerUp Prefabs (Sprint + Slow)")]
    [SerializeField] private GameObject[] powerUpPrefabs;

    [Header("Respawn Settings")]
    public float respawnDelay = 5f; // Pindah ke sini

    private void Start()
    {
        SpawnNewItem();
    }

    public void SpawnNewItem()
    {
        if (powerUpPrefabs.Length == 0) return;

        // Pilih prefab random
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        GameObject spawnedItem = Instantiate(prefab, transform.position, Quaternion.identity);

        // Beritahu item tersebut siapa "induk" (spawner) nya
        var pickupScript = spawnedItem.GetComponent<Main.Gameplay.ItemPickup>();
        if (pickupScript != null)
        {
            pickupScript.SetSpawner(this);
        }
    }

    // Fungsi ini akan dipanggil oleh item saat diambil
    public void StartRespawnTimer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnNewItem(); // Munculkan item baru secara random setelah delay
    }
}