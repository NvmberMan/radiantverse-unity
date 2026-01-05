using UnityEngine;

namespace Main.Gameplay.AI
{
    public class GlobalEnvironment : MonoBehaviour
    {
        public AIInput[] bots;
        public Transform[] targetPoints;

        public static GlobalEnvironment instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
