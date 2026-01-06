using UnityEngine;

namespace Main.Gameplay.AI
{
    public class GlobalEnvironment : MonoBehaviour
    {
        public AIInput[] bots;
        public TargetPoint[] targetPoints;

        public static GlobalEnvironment instance;

        private void Awake()
        {
            instance = this;

            for(int i = 0; i < targetPoints.Length; i++)
            {
                TargetPoint point = targetPoints[i];
                point.targetIndex = i;
            }
        }
    }
}
