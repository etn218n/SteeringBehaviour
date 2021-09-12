using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Align : SteeringBehaviour
    {
        [Header("Reaching Angle")]
        [SerializeField] protected float stoppingRadius;
        [SerializeField] protected float slowdownRadius;


        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var angularAcceleration = CalculateAngularAcceleration(current.Forward, target.Forward, current.AngularVelocity.magnitude, threshold);

            return new SteeringOutput(null, angularAcceleration);
        }
        

        protected Vector3? CalculateAngularAcceleration(Vector3 currentDirection, Vector3 targetDirection, float currentAngularSpeed, SteeringThreshold threshold)
        {
            var rotationAxis   = Vector3.Cross(currentDirection, targetDirection).normalized;
            var remainingAngle = Vector3.SignedAngle(currentDirection, targetDirection, rotationAxis);

            if (Mathf.Abs(remainingAngle) <= stoppingRadius)
                return null;

            stoppingRadius = Mathf.Clamp(stoppingRadius, 1.0f, float.MaxValue);
            slowdownRadius = Mathf.Clamp(slowdownRadius, 1.0f, float.MaxValue);

            var targetAngularSpeed        = Mathf.Abs(remainingAngle) > slowdownRadius ? threshold.MaxAngularSpeed : threshold.MaxAngularSpeed * remainingAngle / slowdownRadius;
            var targetAngularAcceleration = Mathf.Abs(targetAngularSpeed - currentAngularSpeed * Mathf.Rad2Deg) / Time.fixedDeltaTime;

            return rotationAxis * targetAngularAcceleration * Mathf.Deg2Rad;
        }
    }
}
