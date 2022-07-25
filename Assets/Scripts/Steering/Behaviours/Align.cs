using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Align : SteeringBehaviour
    {
        [Header("Reaching Orientation")]
        [SerializeField] private float stoppingAngle = 1f;
        [SerializeField] private float slowdownAngle = 10f;
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var angularAcceleration = Steering.ReachOrientation(current, target, stoppingAngle, slowdownAngle, threshold);
            return new SteeringOutput(null, angularAcceleration);
        }
    }
}
