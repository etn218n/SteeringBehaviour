using Kinematics;
using UnityEngine;

namespace Steering
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        [Header("Blending")]
        [SerializeField] private float weight = 1;

        public float Weight
        {
            get => weight;
            set => weight = value;
        }
        
        public abstract SteeringOutput Steer(Kinematic current, Kinematic target, SteeringThreshold threshold);
    }
}