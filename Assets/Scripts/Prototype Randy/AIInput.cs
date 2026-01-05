using UnityEngine;
using Main.Gameplay;

namespace Main.Gameplay.AI
{
    public class AiInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private WaypointCircuit _currentCircuit; // Masukkan jalur disini

        [Header("Navigation Logic")]
        [SerializeField] private float _reachDistance = 2.0f; // Jarak dianggap "sampai"
        [SerializeField] private float _rotationSpeed = 10f;

        private int _currentIndex = 0;
        private Transform _currentTarget;

        private void Awake()
        {
            if (_movement == null)
                _movement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            // Validasi awal
            if (_currentCircuit != null && _currentCircuit.waypoints.Count > 0)
            {
                _currentTarget = _currentCircuit.waypoints[_currentIndex];
            }
            else
            {
                Debug.LogError("AI tidak punya Circuit Waypoint!");
            }
        }

        private void Update()
        {
            if (_currentTarget == null) return;

            // 1. Cek Jarak
            float distance = Vector3.Distance(transform.position, _currentTarget.position);

            // 2. Jika sudah dekat, ganti ke titik selanjutnya
            if (distance <= _reachDistance)
            {
                NextWaypoint();
            }

            // 3. Gerakkan Bot
            MoveToTarget(_currentTarget.position);
        }

        void NextWaypoint()
        {
            // Cek apakah kita masih punya titik selanjutnya?
            // (Index dimulai dari 0, jadi titik terakhir adalah Count - 1)
            if (_currentIndex < _currentCircuit.waypoints.Count - 1)
            {
                // Lanjut ke titik berikutnya
                _currentIndex++;
                _currentTarget = _currentCircuit.waypoints[_currentIndex];
            }
            else
            {
                // Sudah sampai titik terakhir (Finish)!

                // 1. Hapus target agar Update() berhenti mengejar
                _currentTarget = null;

                // 2. Paksa berhenti total
                _movement.StopMoving();

                Debug.Log("AI Reached Finish Line!");
            }
        }

        public void MoveToTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Tetap di tanah

            _movement.MoveToDir(direction);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
}