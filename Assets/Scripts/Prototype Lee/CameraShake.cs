using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Gameplay
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance;

        private CinemachineImpulseSource impulseSource;
        private Coroutine shakeRoutine;
        private Transform cam;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            impulseSource = GetComponent<CinemachineImpulseSource>();
            cam = Camera.main.transform; // 🔥 penting
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ShakeForDuration(8, 4);
            }
        }

        public void ShakeForDuration(float strength, float duration)
        {
            if (impulseSource == null) return;

            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);

            Debug.Log("Shakinggg in " + duration.ToString());

            shakeRoutine = StartCoroutine(ShakeRoutine(strength, duration));
        }

        private IEnumerator ShakeRoutine(float strength, float duration)
        {
            float elapsed = 0f;
            const float interval = 0.1f;

            while (elapsed < duration)
            {
                // 🔥 SHAKE DI CAMERA SPACE (BUKAN WORLD UP)
                Vector3 shakeDir =
                    cam.right * Random.Range(-1f, 1f) +
                    cam.up * Random.Range(-1f, 1f);

                impulseSource.GenerateImpulse(shakeDir.normalized * strength);

                elapsed += interval;
                yield return new WaitForSecondsRealtime(interval);
            }
        }
    }
}
