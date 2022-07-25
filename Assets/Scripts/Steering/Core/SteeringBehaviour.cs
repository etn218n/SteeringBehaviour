using System;
using Kinematics;
using UnityEngine;

namespace Steering
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        [Header("Blending")]
        [SerializeField] private float weight = 1;

        private Motor motor;
        public  Motor Motor => motor;

        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        private void Awake()
        {
            motor = GetComponent<Motor>();
        }

        public abstract SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold);
    }
}