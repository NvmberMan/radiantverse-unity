using System.Collections.Generic;
using UnityEngine;

namespace Main.Gameplay
{
    public class CharacterSpawn : MonoBehaviour
    {
        public float fallThreshold = -15f;

        // RESPAWN final (dipakai semua)
        public Vector3 startPoint;
        public Vector3 spawnPoint;

        public void SetupStartPoint()
        {
            List<SpawnPoint> availableSpawns = new List<SpawnPoint>();

            foreach (GameObject obj in GameManager.Instance.spawnPoints)
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
                spawnPoint = chosen.transform.position;
                startPoint = chosen.transform.position;
                transform.position = spawnPoint;

                if(GetComponent<ICurriculumLearning>() != null)
                {
                    ICurriculumLearning curriculum = GetComponent<ICurriculumLearning>();
                    curriculum.SetTargetPoint(chosen.nextTargetPointIndex);
                    curriculum.SetFinishPoint(chosen.finishTargetPointIndex);
                }
            }
            else
            {
                // fallback: jika tidak ada spawnpoint
                spawnPoint = transform.position;
                startPoint = transform.position;
            }
        }

        private void Update()
        {
            if (transform.position.y < fallThreshold)
            {
                transform.position = spawnPoint;
            }
        }

        public void SetSpawnPoint(Vector3 newSpawnPoint)
        {
            spawnPoint = newSpawnPoint;
        }

        public void RespawnToCheckpoint()
        {
            transform.position = spawnPoint;
        }
        public void RespawnToStartPoint()
        {
            transform.position = startPoint;
        }
    }
}