using UnityEngine;

namespace Steering
{
    public struct SteeringOutput
    {
        public static SteeringOutput Empty = new SteeringOutput(null, null);

        private Vector3? linearAcceleration;
        private Vector3? angularAcceleration;

        public bool HasLinearAcceleration  => linearAcceleration.HasValue;
        public bool HasAngularAcceleration => angularAcceleration.HasValue;
        public Vector3 LinearAcceleration  => linearAcceleration.Value;
        public Vector3 AngularAcceleration => angularAcceleration.Value;


        public SteeringOutput(Vector3? linearAcceleration, Vector3? angularAcceleration)
        {
            this.linearAcceleration  = linearAcceleration;
            this.angularAcceleration = angularAcceleration;
        }
    }
}