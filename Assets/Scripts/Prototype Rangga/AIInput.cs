using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Main.Gameplay.AI
{
    public class AIInput : Agent, ICurriculumLearning
    {
        public Transform currentTarget;
        public int checkpointIndex;
        public int wayIndex;
        public bool restartToCheckpoint = false;

        [SerializeField] private Transform raySensorRoot;
        [SerializeField] private LayerMask groundMask;

        // --- TAMBAHAN DDA (DYNAMIC DIFFICULTY) ---
        [Header("Dynamic Difficulty")]
        [Range(0f, 1f)]
        public float playerSkillLevel; // 0.0 = Pemain Pemula, 1.0 = Pemain Pro

        private CharacterMovement CharacterMovement;
        private CharacterSpawn CharacterSpawn;
        private float previousDistanceToTarget;
        private Vector3 groundNormal = Vector3.up;

        public override void Initialize()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            CharacterSpawn = GetComponent<CharacterSpawn>();
        }

        public override void OnEpisodeBegin()
        {
            // --- TAMBAHAN DDA ---
            // Saat training, kita acak skill lawan agar AI belajar semua tingkat kesulitan
            playerSkillLevel = UnityEngine.Random.Range(0f, 1f);

            if (GlobalEnvironment.instance == null)
            {
                Debug.LogError("GlobalEnvironment.instance NULL");
                return;
            }

            var env = GlobalEnvironment.instance;

            if (env.ways[wayIndex].targetPoints == null || env.ways[wayIndex].targetPoints.Length == 0)
            {
                Debug.LogError("TargetPoints belum diinisialisasi");
                return;
            }

            if (CharacterSpawn == null)
            {
                Debug.LogError("CharacterSpawn NULL");
                return;
            }

            if (CharacterMovement != null && CharacterMovement.rb != null)
            {
                CharacterMovement.rb.linearVelocity = Vector3.zero;
                CharacterMovement.rb.angularVelocity = Vector3.zero;
            }

            if (restartToCheckpoint)
            {
                CharacterSpawn.RespawnToCheckpoint();
            }
            else
            {
                checkpointIndex = 0;
                CharacterSpawn.RespawnToStartPoint();
            }

            if (env.ways[wayIndex].targetPoints != null && checkpointIndex < env.ways[wayIndex].targetPoints.Length)
            {
                currentTarget = env.ways[wayIndex].targetPoints[checkpointIndex].transform;
            }

            if (currentTarget != null)
            {
                previousDistanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            }
        }

        private void FixedUpdate()
        {
            UpdateGroundNormal();
            AlignRaySensorToGround();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // --- TAMBAHAN DDA ---
            // Observasi 0: Beritahu AI seberapa jago lawannya (1 float)
            sensor.AddObservation(playerSkillLevel);

            // Observasi 1: Status Grounded (1 bool = 1 float)
            sensor.AddObservation(CharacterMovement._isGrounded);

            // Observasi 2: Velocity Lokal (3 float)
            sensor.AddObservation(transform.InverseTransformDirection(CharacterMovement.rb.linearVelocity));

            if (currentTarget != null)
            {
                Vector3 relativeTargetPos = transform.InverseTransformPoint(currentTarget.position);

                // Observasi 3: Arah ke target (3 float)
                sensor.AddObservation(relativeTargetPos.normalized);

                // Observasi 4: Jarak relatif (1 float)
                sensor.AddObservation(relativeTargetPos.magnitude / 20f);
            }
            else
            {
                // Harus balance jumlah observasinya jika target null (3 float + 1 float)
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // Action 0: Jump
            if (actions.DiscreteActions[0] == 1)
            {
                CharacterMovement.Jump();
            }

            // Action 1 & 2: Movement
            Vector3 moveDir = Vector3.zero;
            moveDir.x = actions.DiscreteActions[1] == 1 ? 1 : (actions.DiscreteActions[1] == 2 ? -1 : 0);
            moveDir.z = actions.DiscreteActions[2] == 1 ? 1 : (actions.DiscreteActions[2] == 2 ? -1 : 0);

            if (moveDir.magnitude > 0.1f)
            {
                CharacterMovement.MoveToDir(moveDir.normalized, moveDir.x);
            }
            else
            {
                CharacterMovement.StopMoving();
            }

            // Reward Logic
            float currentDistance = Vector3.Distance(transform.position, currentTarget.position);
            float diff = previousDistanceToTarget - currentDistance;

            if (diff > 0)
            {
                AddReward(diff * 0.8f);
                previousDistanceToTarget = currentDistance;
            }

            // --- TAMBAHAN DDA (Time Penalty) ---
            // Jika lawan pemula (0.0), penalty waktu kecil (-0.001f). 
            // Jika lawan pro (1.0), penalty waktu besar (-0.005f) agar AI ngebut.
            float dynamicTimePenalty = Mathf.Lerp(-0.001f, -0.005f, playerSkillLevel);
            AddReward(dynamicTimePenalty);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;
            discrete[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
            discrete[1] = (int)Input.GetAxisRaw("Horizontal") == 1 ? 1 : ((int)Input.GetAxisRaw("Horizontal") == -1 ? 2 : 0);
            discrete[2] = (int)Input.GetAxisRaw("Vertical") == 1 ? 1 : ((int)Input.GetAxisRaw("Vertical") == -1 ? 2 : 0);
        }

        private void UpdateGroundNormal()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 2f, groundMask))
            {
                groundNormal = hit.normal;
            }
            else
            {
                groundNormal = Vector3.up;
            }
        }

        private void AlignRaySensorToGround()
        {
            Vector3 forwardOnGround = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;

            if (forwardOnGround.sqrMagnitude < 0.001f)
                return;

            raySensorRoot.rotation = Quaternion.LookRotation(forwardOnGround, groundNormal);
        }

        private void AdvanceToNextTarget()
        {
            checkpointIndex++;

            if (checkpointIndex < GlobalEnvironment.instance.ways[wayIndex].targetPoints.Length)
            {
                currentTarget = GlobalEnvironment.instance.ways[wayIndex].targetPoints[checkpointIndex].transform;
                previousDistanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                AddReward(10f);
            }
            else
            {
                AddReward(20f);
                restartToCheckpoint = false;
                checkpointIndex = 0;
                EndEpisode();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                if (RemoteTestManager.Instance != null) RemoteTestManager.Instance.LogDeath();
                AddReward(-1.0f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Wall_low"))
            {
                AddReward(-0.5f);
            }
            else if (other.gameObject.CompareTag("Wall_high"))
            {
                if (RemoteTestManager.Instance != null) RemoteTestManager.Instance.LogDeath();
                AddReward(-2.0f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Sensor_void") || other.gameObject.CompareTag("Car_Move"))
            {
                if (RemoteTestManager.Instance != null) RemoteTestManager.Instance.LogDeath();
                AddReward(-5f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Power_Down"))
            {
                // --- TAMBAHAN DDA (Punishment) ---
                // Lawan pemula = AI kurang peduli kena Power Down (-2f)
                // Lawan pro = AI sangat takut kena Power Down (-5f)
                float dynPunishment = Mathf.Lerp(-2.0f, -5.0f, playerSkillLevel);
                AddReward(dynPunishment);
            }
            else if (other.gameObject.CompareTag("Power_Up"))
            {
                AddReward(5f);
            }
            else if (other.gameObject.CompareTag("TargetPoint"))
            {
                TargetPoint point = other.GetComponent<TargetPoint>();
                if (point != null && point.targetIndex == checkpointIndex) AdvanceToNextTarget();
            }
            else if (other.gameObject.CompareTag("FinishPoint"))
            {
                AdvanceToNextTarget();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                AddReward(-1.0f);
            }
            else if (collision.gameObject.CompareTag("Wall_low"))
            {
                AddReward(-0.5f);
            }
            else if (collision.gameObject.CompareTag("Sensor_void") || collision.gameObject.CompareTag("Car_Move"))
            {
                if (RemoteTestManager.Instance != null) RemoteTestManager.Instance.LogDeath();
                AddReward(-5f);
                EndEpisode();
            }
            else if (collision.gameObject.CompareTag("Power_Down"))
            {
                // --- TAMBAHAN DDA (Punishment) ---
                float dynPunishment = Mathf.Lerp(-2.0f, -5.0f, playerSkillLevel);
                AddReward(dynPunishment);
            }
            else if (collision.gameObject.CompareTag("Power_Up"))
            {
                AddReward(5f);
            }
        }

        public void SetTargetPoint(int target) { }
        public void SetFinishPoint(int target) { }
    }
}