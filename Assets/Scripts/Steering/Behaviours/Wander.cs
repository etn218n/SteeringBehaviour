using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Wander : SteeringBehaviour
    {
        [Range(0.01f, 1.0f)]
        [SerializeField] private float deviation = 0.05f;
        
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var angularAcceleration = threshold.MaxAngularSpeed * Random.Range(-deviation, deviation) * current.Up;
            var linearAcceleration  = current.Forward * threshold.MaxLinearAcceleration;
            
            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }
    }
}