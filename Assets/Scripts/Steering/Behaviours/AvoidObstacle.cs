using System.Collections.Generic;
using Kinematics;
using UnityEngine;

namespace Steering
{
    public class AvoidObstacle : SteeringBehaviour
    {
        [SerializeField] private LayerMask raycastLayer;
        [SerializeField] private int deltaAngle = 10;
        [SerializeField] private float predictionTime;

        protected List<SensorRay> rayList = new List<SensorRay>();
        protected Vector3 debugTargetPosition;
        
        
        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            var stoppingAngle = threshold.MaxAngularSpeed * 0.01f;
            var slowdownAngle = threshold.MaxAngularSpeed * 0.5f;
            
            GenerateSensorFOV(current.Position, (target.Position - current.Position).normalized, current.Up);
            
            var avoidDirection = Vector3.zero;
            
            foreach (var ray in rayList)
            {
                if (Physics.Raycast(ray.Origin, ray.Direction, 2f, raycastLayer))
                {
                    avoidDirection += ray.Direction * ray.Weight;
                    ray.IsHit = true;
                }
            }

            var angularTarget = target;
            angularTarget.Forward = avoidDirection;
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);
            var linearAcceleration = current.Forward * threshold.MaxLinearSpeed;
            
            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }


        private void GenerateSensorFOV(Vector3 origin, Vector3 centerDirection, Vector3 rotationAxis)
        {
            rayList.Clear();

            rayList.Add(new SensorRay(origin, centerDirection, 100f));
            
            for (int currentAngle = deltaAngle; currentAngle <= 90f; currentAngle += deltaAngle)
            {
                var v      = Quaternion.AngleAxis(currentAngle, rotationAxis) * centerDirection;
                var weight = currentAngle / 90f;
                var ray    = new SensorRay(origin, v.normalized, weight);
                
                rayList.Add(ray);
            }

            for (int currentAngle = -deltaAngle; currentAngle >= -90f; currentAngle -= deltaAngle)
            {
                var v      = Quaternion.AngleAxis(currentAngle, rotationAxis) * centerDirection;
                var weight = currentAngle / 90f;
                var ray    = new SensorRay(origin, v.normalized, weight);
                
                rayList.Add(ray);
            }
        }
        
        
        public void OnDrawGizmos()
        {
            foreach (var ray in rayList)
            {
                var color = ray.IsHit ? Color.red : Color.white;
                Gizmos.color = color;
                Gizmos.DrawRay(ray.Origin, ray.Direction * 2f);
            }
        }
    }

    public class SensorRay
    {
        public readonly float Weight;
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;

        public bool IsHit;

        public SensorRay(Vector3 origin, Vector3 direction, float weight)
        {
            Weight    = weight;
            Origin    = origin;
            Direction = direction;
        }
    }
}