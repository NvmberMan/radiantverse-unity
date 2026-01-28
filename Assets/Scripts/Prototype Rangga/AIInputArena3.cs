using NUnit;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Main.Gameplay.AI
{
    public class AIInputArena3 : Agent, ICurriculumLearning
    {
        public Transform currentTarget;
        public int targetPointIndex;

        [SerializeField] private Transform raySensorRoot;
        [SerializeField] private LayerMask groundMask;


        private CharacterMovement CharacterMovement;
        private CharacterSpawn CharacterSpawn;
        private float previousDistanceToTarget;
        private Vector3 groundNormal = Vector3.up;

        GlobalEnvironment env;

        private int startTargetPointIndex;
        private int finishTargetPointIndex;

        public override void Initialize()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            CharacterSpawn = GetComponent<CharacterSpawn>();

            env = GlobalEnvironment.instance;
        }

        public override void OnEpisodeBegin()
        {
            if (GlobalEnvironment.instance == null)
            {
                Debug.LogError("GlobalEnvironment.instance NULL");
                return;
            }

            if (env.targetPoints == null || env.targetPoints.Length == 0)
            {
                Debug.LogError("TargetPoints belum diinisialisasi");
                return;
            }

            if (CharacterSpawn == null)
            {
                Debug.LogError("CharacterSpawn NULL");
                return;
            }

            targetPointIndex = startTargetPointIndex;
            currentTarget = env.targetPoints[startTargetPointIndex].transform;

            CharacterSpawn.RespawnToStartPoint();

            previousDistanceToTarget = Vector3.Distance(
                transform.position,
                currentTarget.position
            );

            Debug.Log($"Targetku adalah {targetPointIndex}");
        }


        private void FixedUpdate()
        {
            UpdateGroundNormal();
            AlignRaySensorToGround();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(CharacterMovement._isGrounded);
            sensor.AddObservation(transform.InverseTransformDirection(CharacterMovement.rb.linearVelocity));

            //sensor.AddObservation(targetPointIndex);

            if (currentTarget != null)
            {
                Vector3 relativeTargetPos = transform.InverseTransformPoint(currentTarget.position);
                sensor.AddObservation(relativeTargetPos);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (actions.DiscreteActions[0] == 1)
            {
                CharacterMovement.Jump();
                AddReward(-0.01f);
            }

            float hInput = actions.DiscreteActions[1] == 1 ? 1f : (actions.DiscreteActions[1] == 2 ? -1f : 0f);
            float vInput = actions.DiscreteActions[2] == 1 ? 1f : (actions.DiscreteActions[2] == 2 ? -1f : 0f);

            Vector3 moveDir = new Vector3(hInput, 0, vInput).normalized;

            if (moveDir.magnitude > 0.1f)
            {
                CharacterMovement.MoveToDir(moveDir, hInput);
            }
            else
            {
                CharacterMovement.StopMoving();
            }

            float currentDistance = Vector3.Distance(transform.position, currentTarget.position);
            float diff = previousDistanceToTarget - currentDistance;

            AddReward(diff * 0.05f);

            if (currentDistance < previousDistanceToTarget)
            {
                previousDistanceToTarget = currentDistance;
            }

            AddReward(-0.002f);
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
            targetPointIndex++;

            if ((targetPointIndex - 1) == finishTargetPointIndex)
            {
                Debug.Log($"Aku menang {targetPointIndex}");
                AddReward(10f);
                EndEpisode();
            }
            else if (targetPointIndex < GlobalEnvironment.instance.targetPoints.Length)
            {
                currentTarget = GlobalEnvironment.instance.targetPoints[targetPointIndex].transform;

                previousDistanceToTarget = Vector3.Distance(
                    transform.position,
                    currentTarget.position
                );

                AddReward(5f);
            }
            else
            {
                AddReward(10f);
                EndEpisode();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Wall")
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (other.gameObject.tag == "Wall_low")
            {
                AddReward(-1f);
                EndEpisode();
            }
            else if (other.gameObject.tag == "Wall_high")
            {
                AddReward(-0.3f);
                EndEpisode();
            }
            else if (other.gameObject.tag == "Sensor_void")
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (other.gameObject.tag == "TargetPoint" || other.gameObject.tag == "FinishPoint")
            {
                TargetPoint point = other.GetComponent<TargetPoint>();

                if (point.targetIndex == targetPointIndex)
                    AdvanceToNextTarget();
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                AddReward(-5f);
                EndEpisode();
            }
            else if (collision.gameObject.tag == "Wall_low")
            {
                AddReward(-1f);
                EndEpisode();
            }
            else if (collision.gameObject.tag == "Sensor_void")
            {
                AddReward(-5f);
                EndEpisode();
            }
        }

        public void SetTargetPoint(int target)
        {
            Debug.Log($"Set awal target adalah {targetPointIndex}");

            startTargetPointIndex = target;
        }

        public void SetFinishPoint(int target)
        {
            finishTargetPointIndex = target;
        }
    }
}
