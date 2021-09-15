using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Face : SteeringBehaviour
    {
        [Header("Reaching Orientation")]
        [SerializeField] private float stoppingAngle = 1f;
        [SerializeField] private float slowdownAngle = 10f;
        
        
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            target.Forward = (target.Position - current.Position).normalized;
            
            var angularAcceleration = Steering.ReachOrientation(current, target, stoppingAngle, slowdownAngle, threshold);
            
            return new SteeringOutput(null, angularAcceleration);
        }
    }
}