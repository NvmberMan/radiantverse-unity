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

            checkpointIndex = 0;
            currentTarget = env.targetPoints[0].transform;

            CharacterSpawn.RespawnToStartPoint();

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
            sensor.AddObservation(CharacterMovement._isGrounded);
            sensor.AddObservation(transform.InverseTransformDirection(CharacterMovement.rb.linearVelocity));

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
            checkpointIndex++;

            if (checkpointIndex < GlobalEnvironment.instance.targetPoints.Length)
            {
                currentTarget = GlobalEnvironment.instance.targetPoints[checkpointIndex].transform;

                previousDistanceToTarget = Vector3.Distance(
                    transform.position,
                    currentTarget.position
                );

                AddReward(5f);
            }
            else
            {
                AddReward(10f);
                //CharacterMovement._isFreeze = true;
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

                if (point.targetIndex == checkpointIndex)
                    AdvanceToNextTarget();
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                AddReward(-5f);
            }
            else if (collision.gameObject.tag == "Wall_low")
            {
                AddReward(-1f);
            }
            else if (collision.gameObject.tag == "Sensor_void")
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
