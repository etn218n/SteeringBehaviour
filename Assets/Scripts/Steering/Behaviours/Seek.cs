using Kinematics;

namespace Steering
{
    public class Seek : SteeringBehaviour
    {
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var seekDirection      = (target.Position - current.Position).normalized;
            var linearAcceleration = seekDirection * threshold.MaxLinearAcceleration;

            return new SteeringOutput(linearAcceleration, null);
        }
    }
}