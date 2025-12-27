using System.Collections.Generic;
using UnityEngine;

namespace Main.Gameplay
{
    public class CharacterSpawn : MonoBehaviour
    {
        public float fallThreshold = -15f;

        // RESPAWN final (dipakai semua)
        private Vector3 spawnPoint;

        void Start()
        {
            // 1. Random Spawn awal game
            GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

            List<SpawnPoint> availableSpawns = new List<SpawnPoint>();

            foreach (GameObject obj in spawnObjects)
            {
                SpawnPoint sp = obj.GetComponent<SpawnPoint>();
                if (sp != null && !sp.isTaken)
                {
                    availableSpawns.Add(sp);
                }
            }

            if (availableSpawns.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawns.Count);

                SpawnPoint chosen = availableSpawns[randomIndex];
                chosen.isTaken = true;

                // ini spawn awal player
                SetSpawnPoint(chosen.transform.position);
                transform.position = spawnPoint;
            }
            else
            {
                // fallback: jika tidak ada spawnpoint
                SetSpawnPoint(transform.position);
            }
        }

        private void Update()
        {
            if (transform.position.y < fallThreshold)
            {
                Respawn();
            }
        }

        public void SetSpawnPoint(Vector3 newSpawnPoint)
        {
            spawnPoint = newSpawnPoint;
        }

        public void Respawn()
        {
            transform.position = spawnPoint;
        }
    }
}