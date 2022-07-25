using Kinematics;

namespace Steering
{
    public class Forward : SteeringBehaviour
    {
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var linearAcceleration = current.Forward * threshold.MaxLinearAcceleration;
            return new SteeringOutput(linearAcceleration, null);
        }
    }
}