using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Reach : SteeringBehaviour
    {
        [Header("Reaching Destination")]
        [SerializeField] private float stoppingRadius = 0.1f;
        [SerializeField] private float slowdownRadius = 1.0f;

        [Header("Debug")]
        [SerializeField] private bool showGizmos;
        
        protected Vector3 debugTargetPosition;


        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var linearTarget = target;
            linearTarget.LinearVelocity = (target.Position - current.Position).normalized * threshold.MaxLinearSpeed;

            var linearAcceleration = Steering.ReachDestination(current, linearTarget, stoppingRadius, slowdownRadius, threshold);

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