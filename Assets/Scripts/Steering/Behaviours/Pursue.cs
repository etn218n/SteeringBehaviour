using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Pursue : SteeringBehaviour
    {
        [Header("Approach Target")] 
        [SerializeField] private float approachRadius = 1f;
        [Range(0f, 180f)]
        [SerializeField] private float approachAngle  = 60f;
        [Range(0f, 1f)]
        [SerializeField] private float extrapolation = 0.5f;
        
        [Header("Reaching Destination")]
        [SerializeField] private float stoppingRadius = 0.1f;
        [SerializeField] private float slowdownRadius = 1.0f;
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var stoppingAngle = threshold.MaxAngularSpeed * 0.01f;
            var slowdownAngle = threshold.MaxAngularSpeed * 0.1f;
            
            var approachPosition = CalculateApproachPosition(current, target);
            
            var linearTarget = target;
            linearTarget.Position = approachPosition;
            linearTarget.LinearVelocity = current.Forward * threshold.MaxLinearSpeed;

            var angularTarget = target;
            angularTarget.Forward = approachPosition - current.Position;

            var linearAcceleration  = Steering.ReachDestination(current, linearTarget, stoppingRadius, slowdownRadius, threshold);
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);
            
            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }
        

        private Vector3 CalculateApproachPosition(in Kinematic current, in Kinematic target)
        {
            var extrapolationTime = Vector3.Distance(current.Position, target.Position) * extrapolation;
            var futurePosition = target.Position + target.LinearVelocity * extrapolationTime;
            var angleSign = Vector3.SignedAngle(target.Forward, current.Position - target.Position, target.Up) < 0 ? -1 : 1;
            var v = Quaternion.AngleAxis(approachAngle * angleSign, target.Up) * target.Forward;

            return futurePosition + v.normalized * approachRadius;
        }
    }
}