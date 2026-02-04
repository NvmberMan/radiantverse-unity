using UnityEngine;

namespace Main.Gameplay
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool isTaken = false;
        public int nextTargetPointIndex = 0;
        public int finishTargetPointIndex = 5;
    }
}