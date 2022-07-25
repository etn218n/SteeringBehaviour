using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Face : SteeringBehaviour
    {
        [Header("Reaching Orientation")]
        [SerializeField] private float stoppingAngle = 1f;
        [SerializeField] private float slowdownAngle = 10f;
        
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var angularTarget = target;
            angularTarget.Forward = (target.Position - current.Position).normalized;
            
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);
            return new SteeringOutput(null, angularAcceleration);
        }
    }
}