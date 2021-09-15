using Kinematics;
using UnityEngine;

namespace Steering
{
    public class StaticSteeringTarget : KinematicEntity
    {
        public override ref readonly Kinematic CurrentKinematic
        {
            get
            {
                currentKinematic.Up              = -transform.forward;
                currentKinematic.Forward         =  transform.up;
                currentKinematic.Position        =  transform.position;
                currentKinematic.Orientation     =  transform.rotation;
                currentKinematic.LinearVelocity  =  Vector3.zero;
                currentKinematic.AngularVelocity =  Vector3.zero;

                return ref currentKinematic;
            }
        }
    }
}