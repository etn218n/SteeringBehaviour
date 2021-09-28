using System;
using UnityEngine;
using System.Collections.Generic;

namespace NavigationPath
{
    public class CatmullRomPath : Path
    {
        [SerializeField] private float alpha;
        [SerializeField] private float tension;
        
        [HideInInspector]
        [SerializeField] public List<Vector3> controlPoints = new List<Vector3>();

        public bool IsEmpty => controlPoints.Count == 0;
        public float Alpha => alpha;
        public float Tension => tension;
        
        public override bool IsClosed => false;

        private void Awake()
        {
            points = GeneratePoints();
            InvokeOnChanged();
        }

        public bool CreateFirstSegment(Vector3 startpoint, Vector3 endpoint)
        {
            if (startpoint == endpoint)
                return false;
            
            var v  = (endpoint - startpoint).normalized;
            var p0 = startpoint + v * 2;
            var p3 = endpoint   + v * 2;
            
            controlPoints.Add(plane.Constrain(p0));
            controlPoints.Add(plane.Constrain(startpoint));
            controlPoints.Add(plane.Constrain(endpoint));
            controlPoints.Add(plane.Constrain(p3));

            points = GeneratePoints();
            InvokeOnChanged();

            return true;
        }

        public void AddSegment(Vector3 endpoint)
        {
            if (controlPoints.Contains(endpoint))
                return;
            
            controlPoints.Insert(controlPoints.Count - 1, plane.Constrain(endpoint));

            points = GeneratePoints();
            InvokeOnChanged();
        }
        
        public void RemoveLastSegment()
        {
            if (controlPoints.Count <= 4)
                return;
            
            controlPoints.RemoveAt(controlPoints.Count - 2);
            
            points = GeneratePoints();
            InvokeOnChanged();
        }

        public void ForEachControlPoint(Action<Vector3, int> action)
        {
            for (int i = 1; i <= controlPoints.Count - 2; i++)
                action(controlPoints[i], i);
        }

        public void MoveControlPoint(int index, Vector3 newPosition)
        {
            if (index < 1 || index > controlPoints.Count - 2)
                return;
            
            controlPoints[index] = plane.Constrain(newPosition);
            
            points = GeneratePoints();
            InvokeOnChanged();
        }

        public override void Clear()
        {
            points.Clear();
            controlPoints.Clear();
            
            InvokeOnChanged();
        }
        
        public List<Vector3> GeneratePoints(float resolution = 10f)
        {
            if (controlPoints.Count < 4)
                return new List<Vector3>();
                
            var newPoints = new List<Vector3> { controlPoints[1] };
            var previousPoint = controlPoints[1];
            var distanceSinceLastEvenPoint = 0f;
            
            spacing = Mathf.Clamp(spacing, 0.01f, float.MaxValue);

            for (int i = 1; i <= controlPoints.Count - 3; i++)
            {
                var p0 = controlPoints[i - 1];
                var p1 = controlPoints[i];
                var p2 = controlPoints[i + 1];
                var p3 = controlPoints[i + 2];
                
                var t0 = 0f;
                var t1 = t0 + Mathf.Pow(Vector3.Distance(p0, p1), alpha);
                var t2 = t1 + Mathf.Pow(Vector3.Distance(p1, p2), alpha);
                var t3 = t2 + Mathf.Pow(Vector3.Distance(p2, p3), alpha);

                var m1 = (1f - tension) * (t2 - t1) * ((p1 - p0) / (t1 - t0) - (p2 - p0) / (t2 - t0) + (p2 - p1) / (t2 - t1));
                var m2 = (1f - tension) * (t2 - t1) * ((p2 - p1) / (t2 - t1) - (p3 - p1) / (t3 - t1) + (p3 - p2) / (t3 - t2));

                var a =  2f * (p1 - p2) + m1 + m2;
                var b = -3f * (p1 - p2) - m1 - m1 - m2;
                var c = m1;
                var d = p1;

                var t = 0f;
                var estimatedSegmentLength = EstimateSegmentLength(a, b, c, d, 0.25f);
                var divisions = Mathf.CeilToInt(estimatedSegmentLength * resolution * 10);
                var stepSize  = 1f / divisions;
                
                while (t < 1f)
                {
                    t += stepSize;
                    
                    var pointOnCurve = (a * t * t * t) + (b * t * t) + (c * t) + d;
                    distanceSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                    while (distanceSinceLastEvenPoint >= spacing)
                    {
                        var overshootDistance = distanceSinceLastEvenPoint - spacing;
                        var newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDistance;
                        
                        newPoints.Add(newEvenlySpacedPoint);

                        distanceSinceLastEvenPoint = overshootDistance;
                        previousPoint = newEvenlySpacedPoint;
                    }

                    previousPoint = pointOnCurve;
                }
                
                newPoints.Add(a + b + c + d); // end point
            }

            return newPoints;
        }

        public float EstimateSegmentLength(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float stepSize)
        {
            var t = stepSize;
            var segmentLength = 0f;
            var previousPoint = d;
            
            while (t < 1f)
            {
                var point = (a * t * t * t) + (b * t * t) + (c * t) + d;

                segmentLength += Vector3.Distance(point, previousPoint);
                previousPoint = point;
                t += stepSize;
            }

            var endPoint = a + b + c + d;
            segmentLength += Vector3.Distance(endPoint, previousPoint);

            return segmentLength;
        }
    }
}
