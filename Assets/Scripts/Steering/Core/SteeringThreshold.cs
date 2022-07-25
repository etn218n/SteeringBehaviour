using System;
using UnityEngine;

namespace Steering
{
    [Serializable]
    public struct SteeringThreshold
    {
        [Header("Moving")]
        public float MaxLinearSpeed;
        public float MaxLinearAcceleration;

        [Header("Turning")]
        public float MaxAngularSpeed;
        public float MaxAngularAcceleration;

        public Vector3 ClampLinearAcceleration(Vector3 linearAcceleration)
        {
            var clampedMagnitude = Mathf.Clamp(linearAcceleration.magnitude, -MaxLinearAcceleration, MaxLinearAcceleration);
            return linearAcceleration.normalized * clampedMagnitude;
        }
        
        public Vector3 ClampAngularAcceleration(Vector3 angularAcceleration)
        {
            var clampedMagnitude = Mathf.Clamp(angularAcceleration.magnitude, -MaxAngularAcceleration, MaxAngularAcceleration);
            return angularAcceleration.normalized * clampedMagnitude;
        }

        public Vector3 ClampVelocity(Vector3 linearVelocity)
        {
            var clampedSpeed = Mathf.Clamp(linearVelocity.magnitude, -MaxLinearSpeed, MaxLinearSpeed);
            return linearVelocity.normalized * clampedSpeed;
        }

        public Vector3 ClampRotation(Vector3 angularVelocity)
        {
            var clampedTurnRate = Mathf.Clamp(angularVelocity.magnitude, -MaxAngularSpeed, MaxAngularSpeed);
            return angularVelocity.normalized * clampedTurnRate;
        }
    }
}