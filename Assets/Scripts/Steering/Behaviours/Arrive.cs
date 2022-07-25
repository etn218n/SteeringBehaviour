using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Arrive : SteeringBehaviour
    {
        [Header("Reaching Destination")]
        [SerializeField] private float stoppingRadius = 0.1f;
        [SerializeField] private float slowdownRadius = 1.0f;
        
        [Header("Reaching Angle")]
        [SerializeField] private float stoppingAngle = 1f;
        [SerializeField] private float slowdownAngle = 10f;

        [Header("Debug")]
        [SerializeField] private bool showGizmos;
        
        protected Vector3 debugTargetPosition;
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var linearTarget = target;
            linearTarget.LinearVelocity = current.Forward * threshold.MaxLinearSpeed;

            var angularTarget = target;
            angularTarget.Forward = (target.Position - current.Position).normalized;

            var linearAcceleration  = Steering.ReachDestination(current, linearTarget, stoppingRadius, slowdownRadius, threshold);
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);
            
            debugTargetPosition = target.Position;

            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }
        
        public void OnDrawGizmos()
        {
            if (!showGizmos) 
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugTargetPosition, slowdownRadius);
        }
    }
}