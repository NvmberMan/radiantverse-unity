using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Main.Gameplay
{
    public class CinemachineDollyCamera : MonoBehaviour
    {
        [Header("References")]
        public CinemachineCamera dollyCamera;
        public Transform player;

        [Header("Follow Settings")]
        [Tooltip("Semakin besar = semakin responsif")]
        public float followSharpness = 8f;

        [Tooltip("Offset maju di spline (0–1)")]
        public float forwardOffset = 0.03f;

        CinemachineSplineDolly dolly;
        float velocity;

        void Awake()
        {
            dolly = dollyCamera.GetComponent<CinemachineSplineDolly>();

            if (!dolly)
                Debug.LogError("❌ CinemachineSplineDolly tidak ditemukan!");
        }

        void LateUpdate()
        {
            if (!dolly || dolly.Spline == null || dolly.Spline.Splines.Count == 0 || !player)
                return;

            var spline = dolly.Spline.Splines[0];

            SplineUtility.GetNearestPoint(
                spline,
                player.position,
                out float3 nearest,
                out float t
            );

            float targetPos = Mathf.Clamp01(t + forwardOffset);

            dolly.CameraPosition = Mathf.SmoothDamp(
                dolly.CameraPosition,
                targetPos,
                ref velocity,
                1f / followSharpness
            );
        }


        /// Dipanggil oleh Trigger
        public void ResetDollyPosition()
        {
            velocity = 0f;
            dolly.CameraPosition = 0f;
        }
    }
}
