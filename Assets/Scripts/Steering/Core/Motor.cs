using Kinematics;
using UnityEngine;
using System.Collections.Generic;

namespace Steering
{
    [RequireComponent(typeof(Rigidbody))]
    public class Motor : KinematicEntity
    {
        [SerializeField] private KinematicEntity target;
        [Space]
        [SerializeField] private SteeringThreshold threshold;
        [Space]
        [SerializeField] private List<SteeringBehaviour> steeringBehaviours;

        private Rigidbody rigidBody;
        

        private void Awake() => rigidBody = GetComponent<Rigidbody>();


        private void FixedUpdate()
        {
            var targetKinematic  = (target == null) ? Kinematic.Empty : target.CurrentKinematic;
            var currentKinematic = CurrentKinematic;

            Steer(currentKinematic, targetKinematic, threshold);
        }
        

        private void Steer(Kinematic currentKinematic, Kinematic targetKinematic, SteeringThreshold threshold)
        {
            var stopTurning = true;
            var stopMoving  = true;

            var accumulatedLinearAcceleration  = Vector3.zero;
            var accumulatedAngularAcceleration = Vector3.zero;

            foreach (var behaviour in steeringBehaviours)
            {
                var steeringOutput = behaviour.Steer(currentKinematic, targetKinematic, threshold);
                
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

            rigidBody.velocity        = stopMoving  ? Vector3.zero : UpdateLinearVelocity(accumulatedLinearAcceleration, threshold);
            rigidBody.angularVelocity = stopTurning ? Vector3.zero : UpdateAngularVelocity(accumulatedAngularAcceleration, threshold);
        }


        private Vector3 UpdateAngularVelocity(Vector3 angularAcceleration, SteeringThreshold threshold)
        {
            var updatedAngularVelocity = rigidBody.angularVelocity + threshold.ClampAngularAcceleration(angularAcceleration) * Time.fixedDeltaTime;
            
            return threshold.ClampRotation(updatedAngularVelocity);
        }
        
        
        private Vector3 UpdateLinearVelocity(Vector3 linearAcceleration, SteeringThreshold threshold)
        {
            var newLinearVelocity = rigidBody.velocity + threshold.ClampLinearAcceleration(linearAcceleration) * Time.fixedDeltaTime;
            
            return threshold.ClampVelocity(newLinearVelocity) ;
        }


        public override Kinematic CurrentKinematic
        {
            get => new Kinematic
            {
                Up              = -transform.forward,
                Forward         =  transform.up,
                Position        =  rigidBody.position,
                Orientation     =  rigidBody.rotation,
                LinearVelocity  =  rigidBody.velocity,
                AngularVelocity =  rigidBody.angularVelocity
            };
        }
    } 
}
