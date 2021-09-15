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
        
        protected Vector3 debugTargetPosition;


        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            target.LinearVelocity = (target.Position - current.Position).normalized * threshold.MaxLinearSpeed;

            var linearAcceleration = Steering.ReachDestination(current, target, stoppingRadius, slowdownRadius, threshold);

            debugTargetPosition = target.Position;

            return new SteeringOutput(linearAcceleration, null);
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