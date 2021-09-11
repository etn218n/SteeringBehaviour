using Kinematics;
using UnityEngine;

namespace Steering
{
    public class SteeringTarget : KinematicEntity
    {
        public override Kinematic CurrentKinematic
        {
            get => new Kinematic 
            {
                Up              = -transform.forward,
                Forward         =  transform.up,
                Position        =  transform.position,
                Orientation     =  transform.rotation,
                LinearVelocity  =  Vector3.zero,
                AngularVelocity =  Vector3.zero
            };
        }
    }
}