using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace Main.Gameplay
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance;

        private CinemachineImpulseSource impulse;
        private Coroutine routine;
        private Transform cam;

        private void Awake()
        {
            Instance = this;
            impulse = GetComponent<CinemachineImpulseSource>();
            cam = Camera.main.transform;
        }

        public void ShakeForSeconds(float strength, float duration)
        {
            StopShake();
            routine = StartCoroutine(ShakeRoutine(strength, duration));
        }

        private IEnumerator ShakeRoutine(float strength, float duration)
        {
            float elapsed = 0f;
            const float interval = 0.1f;

            while (elapsed < duration)
            {
                Vector3 dir =
                    cam.right * Random.Range(-1f, 1f) +
                    cam.up * Random.Range(-1f, 1f);

                impulse.GenerateImpulse(dir.normalized * strength);

                elapsed += interval;
                yield return new WaitForSecondsRealtime(interval);
            }

            routine = null;
        }

        public void StopShake()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
