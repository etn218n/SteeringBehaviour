using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Align : SteeringBehaviour
    {
        public static readonly float MinTimeToReachTarget = 0.01f;
        public static readonly float MaxTimeToReachTarget = 1000f;
        public static readonly float MinSlowRadius        = 0.0001f;
        public static readonly float MaxSlowRadius        = float.MaxValue;
        
        [Header("Angular Target")]
        [SerializeField] protected float targetRadius;
        [SerializeField] protected float slowRadius;
        [SerializeField] protected float timeToReachTarget;

        
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var angularAcceleration = CalculateTurning(current.Forward, target.Forward, current.AngularVelocity.magnitude, threshold);

            return new SteeringOutput(null, angularAcceleration);
        }
        

        protected Vector3? CalculateTurning(Vector3 currentDirection, Vector3 targetDirection, float currentRotationSpeed, SteeringThreshold threshold)
        {
            var currentAngle = Vector3.Angle(currentDirection, targetDirection);
            var rotationAxis = Vector3.Cross(currentDirection, targetDirection).normalized;
            
            if (currentAngle <= targetRadius)
                return null;

            slowRadius        = Mathf.Clamp(slowRadius, MinSlowRadius, MaxSlowRadius);
            timeToReachTarget = Mathf.Clamp(timeToReachTarget, MinTimeToReachTarget, MaxTimeToReachTarget);

            var targetRotationSpeed = (currentAngle > slowRadius) ? threshold.MaxAngularSpeed : threshold.MaxAngularSpeed * (currentAngle / slowRadius);
            var angularAcceleration = Mathf.Abs(targetRotationSpeed - currentRotationSpeed * Mathf.Rad2Deg) / timeToReachTarget;
            
            if (angularAcceleration > threshold.MaxAngularAcceleration)
                angularAcceleration = threshold.MaxAngularAcceleration;
            
            if (Vector3.SignedAngle(currentDirection, targetDirection, rotationAxis) < 0)
                angularAcceleration *= -1;

            return rotationAxis * angularAcceleration * Mathf.Deg2Rad;
        }
    }
}
