using Kinematics;
using UnityEngine;

namespace Steering
{
    public static class Steering
    {
        public static Vector3? ReachDestination(in Kinematic current, in Kinematic target, float stoppingRadius, float slowdownRadius, in SteeringThreshold threshold)
        {
            var distanceToTarget = Vector3.Distance(current.Position, target.Position);
            
            stoppingRadius = Mathf.Clamp(stoppingRadius, 0.01f, float.MaxValue);
            slowdownRadius = Mathf.Clamp(slowdownRadius, 0.01f, float.MaxValue);

            if (distanceToTarget <= stoppingRadius)
                return null;

            var targetSpeed        = threshold.MaxLinearSpeed * (distanceToTarget > slowdownRadius ? 1f : distanceToTarget / slowdownRadius);
            var targetVelocity     = targetSpeed * target.LinearVelocity.normalized;
            var targetAcceleration = (targetVelocity - current.LinearVelocity) / Time.fixedDeltaTime;

            return targetAcceleration; 
        }
        
        public static Vector3? ReachOrientation(in Kinematic current, in Kinematic target, float stoppingAngle, float slowdownAngle, in SteeringThreshold threshold)
        {
            var rotationAxis   = Vector3.Cross(current.Forward, target.Forward).normalized;
            var remainingAngle = Vector3.SignedAngle(current.Forward, target.Forward, rotationAxis);

            remainingAngle = Mathf.Abs(remainingAngle);
            stoppingAngle  = Mathf.Clamp(stoppingAngle, 0.01f, float.MaxValue);
            slowdownAngle  = Mathf.Clamp(slowdownAngle, 0.01f, float.MaxValue);
            
            if (remainingAngle <= stoppingAngle)
                return null;
            
            var targetAngularSpeed        = threshold.MaxAngularSpeed * (remainingAngle > slowdownAngle ? 1f : remainingAngle / slowdownAngle);
            var targetAngularAcceleration = (targetAngularSpeed - current.AngularVelocity.magnitude * Mathf.Rad2Deg) / Time.fixedDeltaTime;

            return rotationAxis * targetAngularAcceleration * Mathf.Deg2Rad;
        }
    }
}