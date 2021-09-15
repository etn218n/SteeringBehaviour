using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Arrive : SteeringBehaviour
    {
        [Header("Reaching Destination")]
        [SerializeField] protected float stoppingPositionalRadius = 0.1f;
        [SerializeField] protected float slowdownPositionalRadius = 1.0f;
        
        [Header("Reaching Angle")]
        [SerializeField] protected float stoppingAngularRadius = 1f;
        [SerializeField] protected float slowdownAngularRadius = 10f;

        [Header("Debug")]
        [SerializeField] protected bool showGizmos;

        protected bool isArrived;
        protected Vector3 debugTargetPosition;

        public bool IsArrived => isArrived;


        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var moveDirection       = (target.Position - current.Position).normalized;

            var linearAcceleration  = CalculateLinearAcceleration(current.Forward, current, target, threshold);
            var angularAcceleration = CalculateAngularAcceleration(current.Forward, moveDirection, current.AngularVelocity.magnitude, threshold);
            
            debugTargetPosition = target.Position;

            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }
        

        protected Vector3? CalculateLinearAcceleration(Vector3 moveDirection, Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            isArrived = false;

            var distanceToTarget = Vector3.Distance(target.Position, current.Position);

            if (distanceToTarget <= stoppingPositionalRadius)
            {
                isArrived = true;
                return null;
            }

            stoppingPositionalRadius = Mathf.Clamp(stoppingPositionalRadius, 0.1f, float.MaxValue);
            slowdownPositionalRadius = Mathf.Clamp(slowdownPositionalRadius, 1.0f, float.MaxValue);

            var targetSpeed        = distanceToTarget > slowdownPositionalRadius ? threshold.MaxLinearSpeed : threshold.MaxLinearSpeed * distanceToTarget / slowdownPositionalRadius;
            var targetVelocity     = targetSpeed * moveDirection;
            var targetAcceleration = (targetVelocity - current.LinearVelocity) / Time.fixedDeltaTime;

            return targetAcceleration; 
        }
        
        
        protected Vector3? CalculateAngularAcceleration(Vector3 currentDirection, Vector3 targetDirection, float currentAngularSpeed, SteeringThreshold threshold)
        {
            var rotationAxis   = Vector3.Cross(currentDirection, targetDirection).normalized;
            var remainingAngle = Vector3.SignedAngle(currentDirection, targetDirection, rotationAxis);

            remainingAngle = Mathf.Abs(remainingAngle);
            
            if (remainingAngle <= stoppingAngularRadius)
                return null;

            stoppingAngularRadius = Mathf.Clamp(stoppingAngularRadius, 1.0f, float.MaxValue);
            slowdownAngularRadius = Mathf.Clamp(slowdownAngularRadius, 1.0f, float.MaxValue);

            var targetAngularSpeed        = remainingAngle > slowdownAngularRadius ? threshold.MaxAngularSpeed : threshold.MaxAngularSpeed * remainingAngle / slowdownAngularRadius;
            var targetAngularAcceleration = (targetAngularSpeed - currentAngularSpeed * Mathf.Rad2Deg) / Time.fixedDeltaTime;

            return rotationAxis * targetAngularAcceleration * Mathf.Deg2Rad;
        }
        

        public void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(debugTargetPosition, slowdownPositionalRadius);
            }
        }
    }
}