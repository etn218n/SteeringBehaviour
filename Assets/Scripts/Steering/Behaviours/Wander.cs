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
        
        
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            if (targetDirection == Vector3.zero)
                RandomizeTargetDirection(current.Forward, current.Up);

            var angularAcceleration = CalculateAngularAcceleration(current.Forward, targetDirection, current.AngularVelocity.magnitude, threshold);
            var linearAcceleration  = current.Forward * threshold.MaxLinearAcceleration;
            
            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }
        

        protected Vector3? CalculateAngularAcceleration(Vector3 currentDirection, Vector3 targetDirection, float currentAngularSpeed, SteeringThreshold threshold)
        {
            var rotationAxis   = Vector3.Cross(currentDirection, targetDirection).normalized;
            var remainingAngle = Vector3.SignedAngle(currentDirection, targetDirection, rotationAxis);

            var stoppingRadius = threshold.MaxAngularSpeed * 0.01f;
            var slowdownRadius = threshold.MaxAngularSpeed * 0.1f;
            
            remainingAngle = Mathf.Abs(remainingAngle);
            
            if (remainingAngle <= stoppingRadius)
            {
                RandomizeTargetDirection(currentDirection, rotationAxis);
                return Vector3.zero;
            }
            
            var targetAngularSpeed        = remainingAngle > slowdownRadius ? threshold.MaxAngularSpeed : threshold.MaxAngularSpeed * remainingAngle / slowdownRadius;
            var targetAngularAcceleration = (targetAngularSpeed - currentAngularSpeed * Mathf.Rad2Deg) / Time.fixedDeltaTime;

            return rotationAxis * targetAngularAcceleration * Mathf.Deg2Rad;
        }
        

        private void RandomizeTargetDirection(Vector3 currentDirection, Vector3 rotationAxis)
        {
            var randomAngle = Random.Range(-deviationAngle, deviationAngle);
            
            targetDirection = Quaternion.AngleAxis(randomAngle, rotationAxis) * currentDirection;
        }
    }
}