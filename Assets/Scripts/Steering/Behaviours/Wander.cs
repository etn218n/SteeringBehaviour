using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Wander : SteeringBehaviour
    {
        [Header("Randomness")]
        [Range(0f, 360f)]
        [SerializeField] private float deviationAngle = 60f;

        private Vector3 targetDirection;
        
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            if (targetDirection == Vector3.zero)
                RandomizeTargetDirection(current.Forward, current.Up);
            
            var stoppingAngle = threshold.MaxAngularSpeed * 0.01f;
            var slowdownAngle = threshold.MaxAngularSpeed * 0.1f;

            var angularTarget = target;
            angularTarget.Forward = targetDirection;
            
            var linearAcceleration  = current.Forward * threshold.MaxLinearAcceleration;
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);

            if (angularAcceleration == null)
            {
                RandomizeTargetDirection(current.Forward, current.Up);
                angularAcceleration = Vector3.zero;
            }

            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }


        private void RandomizeTargetDirection(Vector3 currentDirection, Vector3 rotationAxis)
        {
            var randomAngle = Random.Range(-deviationAngle, deviationAngle);
            
            targetDirection = Quaternion.AngleAxis(randomAngle, rotationAxis) * currentDirection;
        }
    }
}