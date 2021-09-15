using Kinematics;
using UnityEngine;

namespace Steering
{
    public static class Steering
    {
        public static Vector3? ReachDestination(Kinematic current, Kinematic target, float stoppingRadius, float slowdownRadius, SteeringThreshold threshold)
        {
            var distanceToTarget = Vector3.Distance(current.Position, target.Position);
            
            stoppingRadius = Mathf.Clamp(stoppingRadius, 0.1f, float.MaxValue);
            slowdownRadius = Mathf.Clamp(slowdownRadius, 1.0f, float.MaxValue);

            if (distanceToTarget <= stoppingRadius)
                return null;

            var targetSpeed        = distanceToTarget > slowdownRadius ? threshold.MaxLinearSpeed : threshold.MaxLinearSpeed * distanceToTarget / slowdownRadius;
            var targetVelocity     = targetSpeed * target.LinearVelocity.normalized;
            var targetAcceleration = (targetVelocity - current.LinearVelocity) / Time.fixedDeltaTime;

            return targetAcceleration; 
        }
        
        
        public static Vector3? ReachOrientation(Kinematic current, Kinematic target, float stoppingAngle, float slowdownAngle, SteeringThreshold threshold)
        {
            var rotationAxis   = Vector3.Cross(current.Forward, target.Forward).normalized;
            var remainingAngle = Vector3.SignedAngle(current.Forward, target.Forward, rotationAxis);

            remainingAngle = Mathf.Abs(remainingAngle);
            
            stoppingAngle  = Mathf.Clamp(stoppingAngle, 1.0f, float.MaxValue);
            slowdownAngle  = Mathf.Clamp(slowdownAngle, 1.0f, float.MaxValue);
            
            if (remainingAngle <= stoppingAngle)
                return null;
            
            var targetAngularSpeed        = remainingAngle > slowdownAngle ? threshold.MaxAngularSpeed : threshold.MaxAngularSpeed * remainingAngle / slowdownAngle;
            var targetAngularAcceleration = (targetAngularSpeed - current.AngularVelocity.magnitude * Mathf.Rad2Deg) / Time.fixedDeltaTime;

            return rotationAxis * targetAngularAcceleration * Mathf.Deg2Rad;
        }
    }
}