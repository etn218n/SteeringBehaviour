using Kinematics;

namespace Steering
{
    public class Seek : SteeringBehaviour
    {
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var seekDirection      = (target.Position - current.Position).normalized;
            var linearAcceleration = seekDirection * threshold.MaxLinearAcceleration;

            return new SteeringOutput(linearAcceleration, null);
        }
    }
}