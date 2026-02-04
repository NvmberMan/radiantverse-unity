using UnityEngine;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableEntity
    {
        public string name;
        public GameObject prefab;
        [Range(0, 100)]
        public int spawnChance = 50;

        [Tooltip("Isi 0 untuk menggunakan speed bawaan dari Prefab")]
        public float customSpeed = 0f;
    }

    [Header("Entity Settings")]
    public List<SpawnableEntity> entitiesToSpawn;
    public float spawnInterval = 3f;

    [Header("Route Settings")]
    public Transform[] routeWaypoints;
    public Color pathColor = Color.green;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomEntity();
            timer = 0;
        }
    }

    void SpawnRandomEntity()
    {
        if (entitiesToSpawn == null || entitiesToSpawn.Count == 0) return;

        // Pilih entity secara acak berdasarkan bobot
        SpawnableEntity selectedEntity = GetWeightedRandomEntity();

        if (selectedEntity != null && selectedEntity.prefab != null)
        {
            GameObject instance = Instantiate(selectedEntity.prefab, transform.position, transform.rotation);
            AIEntity ai = instance.GetComponent<AIEntity>();

            if (ai != null)
            {
                ai.SetRoute(routeWaypoints);

                // Gunakan custom speed jika nilainya lebih dari 0
                if (selectedEntity.customSpeed > 0)
                {
                    ai.SetSpeed(selectedEntity.customSpeed);
                }
            }
        }
    }

    SpawnableEntity GetWeightedRandomEntity()
    {
        int totalWeight = 0;
        foreach (var entity in entitiesToSpawn)
        {
            totalWeight += entity.spawnChance;
        }

        int randomRoll = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var entity in entitiesToSpawn)
        {
            currentWeight += entity.spawnChance;
            if (randomRoll < currentWeight)
            {
                return entity;
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        // (Visualisasi Gizmos tetap sama seperti sebelumnya)
        if (routeWaypoints == null || routeWaypoints.Length == 0) return;
        Gizmos.color = pathColor;
        for (int i = 0; i < routeWaypoints.Length; i++)
        {
            if (routeWaypoints[i] == null) continue;
            Gizmos.DrawSphere(routeWaypoints[i].position, 0.3f);
            if (i == 0) Gizmos.DrawLine(transform.position, routeWaypoints[i].position);
            else Gizmos.DrawLine(routeWaypoints[i - 1].position, routeWaypoints[i].position);
        }
    }
}