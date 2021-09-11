using Kinematics;

namespace Steering
{
    public class Face : Align
    {
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var targetDirection     = (target.Position - current.Position).normalized;
            var angularAcceleration = CalculateTurning(current.Forward, targetDirection, current.AngularVelocity.magnitude, threshold);
            
            return new SteeringOutput(null, angularAcceleration);
        }
    }
}