using System;
using UnityEngine;

namespace Steering
{
    [Serializable]
    public class PIDController
    {
        [SerializeField] private float pFactor; 
        [SerializeField] private float iFactor;
        [SerializeField] private float dFactor;
        
        private float accumulatedError;
        private float lastError;

        public PIDController() { }
        
        public PIDController(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }
        
        public float Update(float current, float target, float deltaTime)
        {
            float correctiveError = target - current;
            float derivative = (correctiveError - lastError) / deltaTime;
            
            accumulatedError += correctiveError * deltaTime;
            lastError = correctiveError;
            
            return (correctiveError * pFactor) + (accumulatedError * iFactor) + (derivative * dFactor);
        }

        public void Clear()
        {
            lastError = 0;
            accumulatedError = 0;
        }
    }  
}