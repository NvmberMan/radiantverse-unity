using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Main.Gameplay.AI
{
    public class AIInput : Agent
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
            checkpointIndex = 0;
            currentTarget = GlobalEnvironment.instance.targetPoints[0].transform;

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

            Vector3 relativeTargetPos = transform.InverseTransformPoint(currentTarget.position);
            sensor.AddObservation(relativeTargetPos);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (actions.DiscreteActions[0] == 1)
            {
                CharacterMovement.Jump();
                AddReward(-0.01f);
            }

            Vector3 moveDir = Vector3.zero;
            moveDir.x = actions.DiscreteActions[1] == 1 ? 1 : (actions.DiscreteActions[1] == 2 ? -1 : 0);
            moveDir.z = actions.DiscreteActions[2] == 1 ? 1 : (actions.DiscreteActions[2] == 2 ? -1 : 0);

            CharacterMovement.MoveToDir(moveDir.normalized);

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
            else if (other.gameObject.tag == "TargetPoint")
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
        }
    }
}
