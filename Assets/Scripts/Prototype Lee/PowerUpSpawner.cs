using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("All PowerUp Prefabs (Sprint + Slow)")]
    [SerializeField] private GameObject[] powerUpPrefabs; // private + SerializeField juga bisa

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (powerUpPrefabs.Length == 0) return;

        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
