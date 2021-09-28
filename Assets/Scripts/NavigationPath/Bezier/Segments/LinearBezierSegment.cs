using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class LinearBezierSegment : BezierSegment
    {
        public LinearBezierSegment(Vector3 anchorPointA, Vector3 anchorPointB, BezierPath ownerPath) : base(anchorPointA, anchorPointB, ownerPath) { }

        public override float EstimateLength()
        {
            return Vector3.Distance(anchorPointA.Position, anchorPointB.Position);
        }

        public override Vector3 Evaluate(float t)
        {
            var direction = (anchorPointB.Position - anchorPointA.Position).normalized;
            
            return anchorPointA.Position + (direction * t * Vector3.Distance(anchorPointB.Position, anchorPointA.Position));
        }

        public override List<ControlPoint> ControlPoints => new List<ControlPoint>(2) { anchorPointA, anchorPointB };
    }
}