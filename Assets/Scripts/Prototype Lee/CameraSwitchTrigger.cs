using Unity.Cinemachine;
using UnityEngine;

namespace Main.Gameplay
{
    public class CameraSwitchTrigger : MonoBehaviour
    {
        [Header("Cinemachine Cameras")]
        public CinemachineCamera followCamera;
        public CinemachineCamera dollyCamera;

        [Header("Priority")]
        public int followPriority = 5;
        public int dollyPriority = 20;

        CinemachineDollyCamera dollyLogic;

        void Awake()
        {
            dollyLogic = dollyCamera.GetComponent<CinemachineDollyCamera>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Reset dolly saat masuk
            if (dollyLogic != null)
                dollyLogic.ResetDollyPosition();

            followCamera.Priority = followPriority;
            dollyCamera.Priority = dollyPriority;
        }
    }
}
