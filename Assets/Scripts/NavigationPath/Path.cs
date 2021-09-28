using System;
using UnityEngine;
using System.Collections.Generic;

namespace NavigationPath
{
    public abstract class Path : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField] protected Plane plane;
        [SerializeField] protected float spacing = 0.1f;

        protected List<Vector3> points = new List<Vector3>();
        
        public Plane Plane => plane;
        public List<Vector3> Points => points;

        public event Action OnChanged;

        public abstract bool IsClosed { get; }
        public abstract void Clear();
        
        protected void InvokeOnChanged() => OnChanged?.Invoke();

        public virtual int FindNearestPointIndexTo(Vector3 point)
        {
            var nearestPointIndex = 0;
            var nearestDistanceSoFar = Vector3.Distance(point, points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                var newDistance = Vector3.Distance(point, points[i]);
                
                if (newDistance < nearestDistanceSoFar)
                {
                    nearestPointIndex = i;
                    nearestDistanceSoFar = newDistance;
                }
            }

            return nearestPointIndex;
        }

        public virtual Vector3 FindNearestPointTo(Vector3 point)
        {
            var nearestPointIndex = FindNearestPointIndexTo(point);
            
            return points[nearestPointIndex];
        }

        public (List<Vector3>, List<Vector3>) GenerateLanePoints(int startIndex, int endIndex, float laneWidth)
        {
            if (laneWidth == 0)
                return (new List<Vector3>(), new List<Vector3>());

            var leftLane  = new List<Vector3>();
            var rightLane = new List<Vector3>();

            startIndex = Mathf.Clamp(startIndex, 0, points.Count - 4);
            endIndex   = Mathf.Clamp(endIndex, 0, points.Count - 4);

            for (int i = startIndex; i <= endIndex; i++)
            {
                var v = points[i + 1] - points[i];
                
                var leftLanePoint  = Vector3.Cross(v.normalized, plane.Normal) * laneWidth / 2 + points[i];
                var rightLanePoint = -Vector3.Cross(v.normalized, plane.Normal) * laneWidth / 2 + points[i];

                leftLane.Add(leftLanePoint);
                rightLane.Add(rightLanePoint);
            }

            return (leftLane, rightLane);
        }
    }
}