
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Gameplay
{
    public class CinemachineDollyFromInputAndSpeed : MonoBehaviour
    {
        [Header("Camera References")]
        public CinemachineCamera dollyCam;
        public CinemachineCamera followCam;

        [Header("Player References")]
        public CharacterMovement playerMovement;
        public Rigidbody playerRb;

        [Header("Movement Settings")]
        [Tooltip("Pengali kecepatan kamera")]
        public float speedMultiplier = 1f;

        [Tooltip("Pengaruh kamera saat di udara")]
        public float airMultiplier = 0.6f;

        [Header("Spline Settings")]
        public bool clampAtEnd = true;

        CinemachineSplineDolly dolly;

        void Awake()
        {
            dolly = dollyCam.GetComponent<CinemachineSplineDolly>();
        }

        void LateUpdate()
        {
            // Dolly hanya jalan jika kamera ini aktif
            if (dollyCam.Priority <= followCam.Priority) return;

            // Ambil input maju/mundur
            float inputZ = Input.GetAxisRaw("Vertical");

            // Kalau tidak ada input → kamera stop
            if (Mathf.Approximately(inputZ, 0f)) return;

            // Ambil speed player (horizontal saja)
            Vector3 vel = playerRb.linearVelocity;
            vel.y = 0f;
            float playerSpeed = vel.magnitude;

            // Saat di udara → kamera lebih lembut
            if (!playerMovement._isGrounded)
                playerSpeed *= airMultiplier;

            // Gerakkan kamera di spline
            dolly.CameraPosition +=
                inputZ * playerSpeed * speedMultiplier * Time.deltaTime;

            // Clamp & cek akhir spline
            if (clampAtEnd)
            {
                if (dolly.CameraPosition >= 1f)
                {
                    dolly.CameraPosition = 1f;
                    SwitchToFollowCamera();
                }
                else
                {
                    dolly.CameraPosition = Mathf.Clamp01(dolly.CameraPosition);
                }
            }
        }

        void SwitchToFollowCamera()
        {
            followCam.Priority = 20;
            dollyCam.Priority = 10;
        }
    }
}
