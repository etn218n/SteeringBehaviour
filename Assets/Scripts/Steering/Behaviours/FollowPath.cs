using Kinematics;
using UnityEngine;
using NavigationPath;

namespace Steering
{
    public class FollowPath : SteeringBehaviour
    {
        [Header("Path & Prediction")] 
        [SerializeField] private Path path;
        [SerializeField] private float predictionTime;
        [SerializeField] private float pointOnPathOffset;
        
        [Header("Reaching Destination")]
        [SerializeField] private float stoppingRadius = 0.1f;
        [SerializeField] private float slowdownRadius = 1.0f;
        
        [Header("Reaching Angle")]
        [SerializeField] private float stoppingAngle = 1f;
        [SerializeField] private float slowdownAngle = 10f;

        [Header("Debug")]
        [SerializeField] private bool showGizmos;

        private Vector3 debugPointOnPath;

        public override SteeringOutput Steer(in Kinematic current, in Kinematic target, in SteeringThreshold threshold)
        {
            if (path == null || path.Points.Count < 2)
                return SteeringOutput.Empty;

            var targetPoint = FindNextPointOnPath(current);
            debugPointOnPath = targetPoint;
            
            var linearTarget = target;
            linearTarget.Position = targetPoint;
            linearTarget.LinearVelocity = current.Forward * threshold.MaxLinearSpeed;

            var angularTarget = target;
            angularTarget.Forward = (targetPoint - current.Position).normalized;

            var linearAcceleration  = Steering.ReachDestination(current, linearTarget, stoppingRadius, slowdownRadius, threshold);
            var angularAcceleration = Steering.ReachOrientation(current, angularTarget, stoppingAngle, slowdownAngle, threshold);

            return new SteeringOutput(linearAcceleration, angularAcceleration);
        }

        private Vector3 FindNextPointOnPath(in Kinematic current)
        {   
            var futurePosition    = current.Position + (current.LinearVelocity * predictionTime);
            var nearestPointIndex = path.FindNearestPointIndexTo(futurePosition);

            var lastPointIndex = path.Points.Count - 1;
            
            if (nearestPointIndex == lastPointIndex)
                return path.Points[lastPointIndex];

            // if (Vector3.Distance(current.Position, path.Points[nearestPointIndex]) < stoppingRadius)
            //     return path.Points[lastPointIndex];

            nearestPointIndex = Mathf.Clamp(nearestPointIndex, 0, lastPointIndex - 1);

            var nearestPoint = path.Points[nearestPointIndex];
            var nextPoint    = path.Points[nearestPointIndex + 1];

            var pointOnPath = Vector3.Project((futurePosition - nearestPoint), (nextPoint - nearestPoint)) + nearestPoint;
            pointOnPath += (nextPoint - nearestPoint).normalized * pointOnPathOffset;

            return pointOnPath;
        }

        public void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(debugPointOnPath, 0.1f);
            }
        }
    }
}