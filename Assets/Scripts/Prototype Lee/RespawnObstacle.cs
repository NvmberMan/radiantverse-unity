using UnityEngine;

namespace Main.Gameplay
{
    public class RespawnObstacle : MonoBehaviour, IObstacleBehavior
    {
        public void OnPlayerHit(GameObject player)
        {
            CharacterSpawn spawn = player.GetComponent<CharacterSpawn>();
            if (spawn != null)
            {
                spawn.Respawn();
            }
        }
    }
}
