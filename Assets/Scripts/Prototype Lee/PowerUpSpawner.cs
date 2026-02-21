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
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("All PowerUp Prefabs (Sprint + Slow)")]
    [SerializeField] private GameObject[] powerUpPrefabs;

    [Header("Respawn Settings")]
    public float respawnDelay = 10f; // Waktu tunggu JIKA item SUDAH diambil
    public float itemLifetime = 15f; // Waktu maksimal item bertahan jika TIDAK diambil

    private GameObject currentItem;      // Menyimpan data item yang sedang muncul di map
    private Coroutine lifetimeCoroutine; // Menyimpan proses timer kedaluwarsa

    private void Start()
    {
        SpawnNewItem();
    }

    public void SpawnNewItem()
    {
        if (powerUpPrefabs.Length == 0) return;

        // 1. Bersihkan item lama jika masih ada (karena kedaluwarsa)
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        // 2. Pilih dan munculkan item baru secara random
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        currentItem = Instantiate(prefab, transform.position, Quaternion.identity);

        // 3. Beritahu item tersebut siapa "induk" (spawner) nya
        var pickupScript = currentItem.GetComponent<Main.Gameplay.ItemPickup>();
        if (pickupScript != null)
        {
            pickupScript.SetSpawner(this);
        }

        // 4. Mulai / Reset Timer Kedaluwarsa
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
        }
        lifetimeCoroutine = StartCoroutine(ItemLifetimeRoutine());
    }

    // --- COROUTINE 1: JIKA ITEM DICUEKIN ---
    private IEnumerator ItemLifetimeRoutine()
    {
        yield return new WaitForSeconds(itemLifetime); // Tunggu 15 detik

        // Waktu habis! Ganti itemnya secara otomatis
        SpawnNewItem();
    }

    // --- COROUTINE 2: JIKA ITEM DIAMBIL ---
    // Fungsi ini akan dipanggil oleh script ItemPickup saat tersentuh player/bot
    public void StartRespawnTimer()
    {
        // PENTING: Matikan timer kedaluwarsa karena item SUDAH diambil
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
        }

        // Mulai hitung mundur 10 detik untuk memunculkan item baru
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay); // Tunggu 10 detik
        SpawnNewItem();
    }
}