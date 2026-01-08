using UnityEngine;

namespace Main.Gameplay
{
    public class LookAtCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool lockX = false;
        [SerializeField] private bool lockY = false;
        [SerializeField] private bool lockZ = false;

        void LateUpdate()
        {
            if (Camera.main == null) return;

            Vector3 dir = transform.position - Camera.main.transform.position;

            if (lockX) dir.x = 0;
            if (lockY) dir.y = 0;
            if (lockZ) dir.z = 0;

            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}