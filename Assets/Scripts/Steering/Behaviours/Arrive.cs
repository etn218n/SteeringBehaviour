using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Arrive : SteeringBehaviour
    {
        [Header("Reaching Destination")]
        [SerializeField] protected float stoppingRadius = 0.1f;
        [SerializeField] protected float slowdownRadius = 1.0f;
        
        [Header("Reaching Angle")]
        [SerializeField] protected float stoppingAngle = 1f;
        [SerializeField] protected float slowdownAngle = 10f;

        [Header("Debug")]
        [SerializeField] protected bool showGizmos;
        
        protected Vector3 debugTargetPosition;


        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            target.Forward = (target.Position - current.Position).normalized;
            target.LinearVelocity = current.Forward * threshold.MaxLinearSpeed;

            var linearAcceleration  = Steering.ReachDestination(current, target, stoppingRadius, slowdownRadius, threshold);
            var angularAcceleration = Steering.ReachOrientation(current, target, stoppingAngle, slowdownAngle, threshold);
            
            debugTargetPosition = target.Position;

            return new SteeringOutput(linearAcceleration, angularAcceleration);
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