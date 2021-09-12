using Kinematics;

namespace Steering
{
    public class Forward : SteeringBehaviour
    {
        public override SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold)
        {
            var linearAcceleration = current.Forward * threshold.MaxLinearAcceleration;

            return new SteeringOutput(linearAcceleration, null);
        }
    }
}