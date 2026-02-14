using UnityEngine;
using System.Collections.Generic;
using Main.Gameplay.AI;

namespace Main.Gameplay
{
    public class Checkpoint : MonoBehaviour
    {
        private HashSet<GameObject> visitors = new HashSet<GameObject>();
        public int nextTargetPoint;
        public int nextWayPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("NPC"))
            {
                if (other.CompareTag("NPC"))
                    other.GetComponent<AIInput>().checkpointIndex = nextTargetPoint;

                if (!visitors.Contains(other.gameObject))
                {
                    RegisterCheckpoint(other.gameObject);
                }
            }
        }

        private void RegisterCheckpoint(GameObject character)
        {
            CharacterSpawn spawner = character.GetComponent<CharacterSpawn>();
            RacerProgress racerProgress = character.GetComponent<RacerProgress>();

            if (spawner != null)
            {
                spawner.SetSpawnPoint(transform.position);

                visitors.Add(character);

                Debug.Log($"{character.name} (Tag: {character.tag}) berhasil ambil checkpoint.");
            }

            if(racerProgress != null)
            {
                racerProgress.resetWayPointIndex = nextWayPoint;
            }
        }
    }
}