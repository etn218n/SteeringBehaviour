using Kinematics;

namespace Steering
{
    public class Flee : SteeringBehaviour
    {
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var fleeDirection      = (current.Position - target.Position).normalized;
            var linearAcceleration = fleeDirection * threshold.MaxLinearAcceleration;

            return new SteeringOutput(linearAcceleration, null);
        }
    }
}