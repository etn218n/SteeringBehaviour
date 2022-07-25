using Kinematics;
using UnityEngine;
using System.Collections.Generic;

namespace Steering
{
    [RequireComponent(typeof(Rigidbody))]
    public class Motor : KinematicEntity
    {
        [SerializeField] private KinematicEntity targetEntity;
        [SerializeField] private SteeringThreshold threshold;
        [SerializeField] private List<SteeringBehaviour> steeringBehaviours;

        private Rigidbody rigidBody;

        private void Awake() => rigidBody = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            var target  = targetEntity == null ? Kinematic.Empty : targetEntity.CurrentKinematic;
            var current = CurrentKinematic;

            Actuate(in current, in target, in threshold);
        }

        private void Actuate(in Kinematic currentKinematic, in Kinematic targetKinematic, in SteeringThreshold threshold)
        {
            var stopTurning = true;
            var stopMoving  = true;

            var accumulatedLinearAcceleration  = Vector3.zero;
            var accumulatedAngularAcceleration = Vector3.zero;

            foreach (var behaviour in steeringBehaviours)
            {
                var steeringOutput = behaviour.Steer(in currentKinematic, in targetKinematic, in threshold);
                
                if (steeringOutput.HasLinearAcceleration)
                {
                    accumulatedLinearAcceleration += steeringOutput.LinearAcceleration * behaviour.Weight;
                    stopMoving = false;
                }
                
                if (steeringOutput.HasAngularAcceleration)
                {
                    accumulatedAngularAcceleration += steeringOutput.AngularAcceleration * behaviour.Weight;
                    stopTurning = false;
                }
            }

            rigidBody.angularVelocity = stopTurning ? Vector3.zero : UpdateAngularVelocity(accumulatedAngularAcceleration, threshold);
            rigidBody.velocity        = stopMoving  ? Vector3.zero : UpdateLinearVelocity(accumulatedLinearAcceleration, threshold);
        }

        private Vector3 UpdateLinearVelocity(Vector3 linearAcceleration, SteeringThreshold threshold)
        {
            var updatedLinearVelocity = rigidBody.velocity + threshold.ClampLinearAcceleration(linearAcceleration) * Time.fixedDeltaTime;
            return threshold.ClampVelocity(updatedLinearVelocity);
        }

        private Vector3 UpdateAngularVelocity(Vector3 angularAcceleration, SteeringThreshold threshold)
        {
            var updatedAngularVelocity = rigidBody.angularVelocity + threshold.ClampAngularAcceleration(angularAcceleration) * Time.fixedDeltaTime;
            return threshold.ClampRotation(updatedAngularVelocity);
        }

        public override ref readonly Kinematic CurrentKinematic
        {
            get
            {
                currentKinematic.Up              = -transform.forward;
                currentKinematic.Forward         =  transform.up;
                currentKinematic.Position        =  rigidBody.position;
                currentKinematic.Orientation     =  rigidBody.rotation;
                currentKinematic.LinearVelocity  =  rigidBody.velocity;
                currentKinematic.AngularVelocity =  rigidBody.angularVelocity;
                return ref currentKinematic;
            }
        }
    } 
}
