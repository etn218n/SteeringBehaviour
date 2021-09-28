using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    public class BezierPath : Path
    {
        [HideInInspector]
        [SerializeField] private int length;

        [HideInInspector] 
        [SerializeReference] private BezierSegment headSegment;
        [HideInInspector]
        [SerializeReference] private BezierSegment tailSegment;
        
        private bool isRunTime;

        public int Length => length;
        public BezierSegment HeadSegment => headSegment;
        public BezierSegment TailSegment => tailSegment;
        public override bool IsClosed => (length >= 2) && (headSegment.PreviousSegment == tailSegment);

        private void Awake()
        {
            isRunTime = true;

            RegeneratePoints();
        }

        public QuadraticBezierSegment AddQuadraticBezierSegment(Vector3 endPoint)
        {
            var startPoint = tailSegment.AnchorPointB.Position;
            var lerpPoint  = (startPoint + endPoint) / 2;
            var extendedSegment = new QuadraticBezierSegment(startPoint, endPoint, lerpPoint, this);
            extendedSegment.SetPlane(plane);
            
            tailSegment.NextSegment = extendedSegment;
            extendedSegment.PreviousSegment = tailSegment;
            
            tailSegment = extendedSegment;
            length++;

            RegeneratePoints();

            return extendedSegment;
        }
        
        public QuadraticBezierSegment CreateFirstQuadraticBezierSegment(Vector3 startPoint, Vector3 endPoint)
        {
            Clear();
            
            var lerpPoint = (startPoint + endPoint) / 2;
            var firstSegment = new QuadraticBezierSegment(startPoint, endPoint, lerpPoint, this);
            firstSegment.SetPlane(plane);

            headSegment = firstSegment;
            tailSegment = firstSegment;
            length++;
            
            RegeneratePoints();

            return firstSegment;
        }
        
        public CubicBezierSegment AddCubicBezierSegment(Vector3 endPoint)
        {
            var startPoint = tailSegment.AnchorPointB.Position;
            
            var lerpPointA = startPoint + Vector3.left  * Mathf.Abs(startPoint.x) * 0.5f;
            var lerpPointB = endPoint   + Vector3.right * Mathf.Abs(startPoint.x) * 0.5f;
            var extendedSegment = new CubicBezierSegment(startPoint, endPoint, lerpPointA, lerpPointB, this);
            extendedSegment.SetPlane(plane);

            tailSegment.NextSegment = extendedSegment;
            extendedSegment.PreviousSegment = tailSegment;
            
            tailSegment = extendedSegment;
            length++;

            RegeneratePoints();

            return extendedSegment;
        }

        public CubicBezierSegment CreateFirstCubicBezierSegment(Vector3 startPoint, Vector3 endPoint)
        {
            Clear();

            var lerpPointA = startPoint + Vector3.left  * Mathf.Abs(startPoint.x) * 0.5f;
            var lerpPointB = endPoint   + Vector3.right * Mathf.Abs(startPoint.x) * 0.5f;
            var firstSegment = new CubicBezierSegment(startPoint, endPoint, lerpPointA, lerpPointB, this);
            firstSegment.SetPlane(plane);
            
            headSegment = firstSegment;
            tailSegment = firstSegment;
            length++;
            
            RegeneratePoints();
            
            return firstSegment;
        }

        public LinearBezierSegment AddLinearBezierSegment(Vector3 endPoint)
        {
            var startPoint = tailSegment.AnchorPointB.Position;
            var extendedSegment = new LinearBezierSegment(startPoint, endPoint, this);
            extendedSegment.SetPlane(plane);
            
            tailSegment.NextSegment = extendedSegment;
            extendedSegment.PreviousSegment = tailSegment;
            
            tailSegment = extendedSegment;
            length++;
            
            RegeneratePoints();

            return extendedSegment;
        }

        public LinearBezierSegment CreateFirstLinearBezierSegment(Vector3 startPoint, Vector3 endPoint)
        {
            Clear();

            var firstSegment = new LinearBezierSegment(startPoint, endPoint, this);
            firstSegment.SetPlane(plane);
            
            headSegment = firstSegment;
            tailSegment = firstSegment;
            length++;

            RegeneratePoints();

            return firstSegment;
        }

        public void RemoveLastSegment()
        {
            if (length <= 1)
                return;
            
            if (IsClosed)
                Break();

            tailSegment = tailSegment.PreviousSegment;
            tailSegment.NextSegment = null;
            length--;
            
            RegeneratePoints();
        }

        public void RegeneratePoints()
        {
            points = GeneratePoints();
            
            InvokeOnChanged();
        }
        
        public List<Vector3> GeneratePoints(float resolution = 1)
        {
            if (!isRunTime)
                return new List<Vector3>();
            
            var newPoints = new List<Vector3> { headSegment.AnchorPointA.Position };
            var previousPoint = headSegment.AnchorPointA.Position;
            var distanceSinceLastEvenPoint = 0f;
            
            spacing = Mathf.Clamp(spacing, 0.01f, float.MaxValue);

            ForeachSegment(segment =>
            {
                var t = 0f;
                var estimatedSegmentLength = segment.EstimateLength();
                var divisions = Mathf.CeilToInt(estimatedSegmentLength * resolution * 10);
                var stepSize  = 1f / divisions;
                
                while (t < 1f)
                {
                    t += stepSize;
                    
                    var pointOnCurve = segment.Evaluate(t);
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
                
                newPoints.Add(segment.AnchorPointB.Position);
            });

            return newPoints;
        }

        public void ForeachSegment(Action<BezierSegment> action)
        {
            var currentSegment = headSegment;
            
            for (int i = 0; i < length; i++)
            {
                action(currentSegment);
                currentSegment = currentSegment.NextSegment;
            }
        }

        public void ForeachControlPoint(Action<ControlPoint> action)
        {
            var currentSegment = headSegment;
            
            for (int i = 0; i < length; i++)
            {
                currentSegment.ControlPoints.ForEach(c => action(c));
                currentSegment = currentSegment.NextSegment;
            }
        }

        public void Close()
        {
            if (IsClosed)
                return;

            tailSegment.AnchorPointB.Position = headSegment.AnchorPointA.Position;
            headSegment.PreviousSegment = tailSegment;
            tailSegment.NextSegment = headSegment;
            
            RegeneratePoints();
        }
        
        public void Break()
        {
            if (!IsClosed)
                return;

            tailSegment.NextSegment = null;
            headSegment.PreviousSegment = null;

            var newPosition = headSegment.AnchorPointA.Position + Vector3.one;

            tailSegment.AnchorPointB.Position = newPosition;
            
            RegeneratePoints();
        }

        public override void Clear()
        {
            length = 0;
            
            points.Clear();
            
            headSegment = null;
            tailSegment = null;
            
            InvokeOnChanged();
        }
    }
}