using System;
using Kinematics;
using UnityEngine;

namespace Steering
{
    public class Separation : SteeringBehaviour
    {
        [SerializeField] private KinematicCollection agents;
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var linearVelocitySum = Vector3.zero;
            
            foreach (var agent in agents)
            {
                if (ReferenceEquals(this.Motor, agent))
                    continue;

                linearVelocitySum += agent.CurrentKinematic.LinearVelocity;
            }

            return new SteeringOutput(linearVelocitySum, null);
        }
    }
}