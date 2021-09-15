using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Reach : SteeringBehaviour
    {
        [Header("Reaching Destination")]
        [SerializeField] protected float stoppingRadius = 0.1f;
        [SerializeField] protected float slowdownRadius = 1.0f;

        [Header("Debug")]
        [SerializeField] protected bool showGizmos;

        protected bool isArrived;
        protected Vector3 debugTargetPosition;

        public bool IsArrived => isArrived;


        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var moveDirection      = (target.Position - current.Position).normalized;
            var linearAcceleration = CalculateLinearAcceleration(moveDirection, current, target, threshold);
            
            debugTargetPosition = target.Position;

            return new SteeringOutput(linearAcceleration, null);
        }
        

        protected Vector3? CalculateLinearAcceleration(Vector3 moveDirection, Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            isArrived = false;

            var distanceToTarget = Vector3.Distance(target.Position, current.Position);

            if (distanceToTarget <= stoppingRadius)
            {
                isArrived = true;
                return null;
            }

            stoppingRadius = Mathf.Clamp(stoppingRadius, 0.1f, float.MaxValue);
            slowdownRadius = Mathf.Clamp(slowdownRadius, 1.0f, float.MaxValue);

            var targetSpeed        = distanceToTarget > slowdownRadius ? threshold.MaxLinearSpeed : threshold.MaxLinearSpeed * distanceToTarget / slowdownRadius;
            var targetVelocity     = targetSpeed * moveDirection;
            var targetAcceleration = (targetVelocity - current.LinearVelocity) / Time.fixedDeltaTime;

            return targetAcceleration; 
        }
        

        public void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(debugTargetPosition, slowdownRadius);
            }
        }
    }
}