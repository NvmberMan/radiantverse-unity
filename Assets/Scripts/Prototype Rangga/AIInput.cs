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



            if(restartToCheckpoint) {
                CharacterSpawn.RespawnToCheckpoint();
            }
            else
            {
                checkpointIndex = 0;
                CharacterSpawn.RespawnToStartPoint();
            }
            currentTarget = env.ways[wayIndex].targetPoints[checkpointIndex].transform;

            previousDistanceToTarget = Vector3.Distance(
                    transform.position,
                    currentTarget.position
                );
        }


        private void FixedUpdate()
        {
            UpdateGroundNormal();
            AlignRaySensorToGround();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Observasi 1: Status Grounded (1 bool)
            sensor.AddObservation(CharacterMovement._isGrounded);

            // Observasi 2: Velocity Lokal (3 float)
            // Menggunakan InverseTransformDirection agar AI tahu kecepatannya relatif terhadap arah hadapnya
            sensor.AddObservation(transform.InverseTransformDirection(CharacterMovement.rb.linearVelocity));

            if (currentTarget != null)
            {
                Vector3 relativeTargetPos = transform.InverseTransformPoint(currentTarget.position);

                // Observasi 3: Arah ke target yang dinormalisasi (Sangat membantu GAIL/Demo)
                sensor.AddObservation(relativeTargetPos.normalized);

                // Observasi 4: Jarak relatif (diskalakan agar nilainya tidak terlalu besar/outlier)
                sensor.AddObservation(relativeTargetPos.magnitude / 20f);
            }
            else
            {
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

            // Berikan reward jika mendekat ke target
            if (diff > 0)
            {
                AddReward(diff * 0.8f);
                previousDistanceToTarget = currentDistance;
            }

            // Time Penalty (Agar AI efisien dan tidak diam saja)
            // Nilainya kecil agar tidak mematikan motivasi eksplorasi
            AddReward(-0.001f);
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
            if (Physics.Raycast(
                transform.position + Vector3.up * 0.2f,
                Vector3.down,
                out RaycastHit hit,
                2f,
                groundMask))
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
            Vector3 forwardOnGround =
                Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;

            if (forwardOnGround.sqrMagnitude < 0.001f)
                return;

            raySensorRoot.rotation = Quaternion.LookRotation(
                forwardOnGround,
                groundNormal
            );
        }

        private void AdvanceToNextTarget()
        {
            checkpointIndex++;

            if (checkpointIndex < GlobalEnvironment.instance.ways[wayIndex].targetPoints.Length)
            {
                currentTarget = GlobalEnvironment.instance.ways[wayIndex].targetPoints[checkpointIndex].transform;

                previousDistanceToTarget = Vector3.Distance(
                    transform.position,
                    currentTarget.position
                );

                // Reward besar karena berhasil mencapai checkpoint
                AddReward(10f);
            }
            else
            {
                // Reward finish
                AddReward(20f);
                EndEpisode();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                AddReward(-1.0f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Wall_low"))
            {
                AddReward(-0.5f);
            }
            else if (other.gameObject.CompareTag("Wall_high"))
            {
                AddReward(-2.0f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Sensor_void"))
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("Car_Move"))
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (other.gameObject.CompareTag("TargetPoint") || other.gameObject.CompareTag("FinishPoint"))
            {
                TargetPoint point = other.GetComponent<TargetPoint>();

                if (point != null && point.targetIndex == checkpointIndex)
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
            else if (collision.gameObject.tag == "Sensor_void")
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (collision.gameObject.CompareTag("Car_Move"))
            {
                AddReward(-5f);
                EndEpisode();
            }
        }

        public void SetTargetPoint(int target)
        {
            //checkpointIndex = target;
        }

        public void SetFinishPoint(int target)
        {
            //throw new System.NotImplementedException();
        }
    }
}