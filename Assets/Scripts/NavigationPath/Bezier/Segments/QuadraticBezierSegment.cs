using UnityEngine;
using System.Collections.Generic;

namespace NavigationPath
{
    public class QuadraticBezierSegment : BezierSegment
    {
        [SerializeReference] private ControlPoint lerpPoint;

        public ControlPoint LerpPoint => lerpPoint;
        
        public override List<ControlPoint> ControlPoints => new List<ControlPoint>(3) { anchorPointA, anchorPointB, lerpPoint };

        public QuadraticBezierSegment(Vector3 anchorPointA, Vector3 anchorPointB, Vector3 lerpPoint, BezierPath ownerPath) 
            : base(anchorPointA, anchorPointB, ownerPath)
        {
            this.lerpPoint = new ControlPoint(lerpPoint, this);
        }
        
        public override void OnControlPointMoved(ControlPoint controlPoint, Vector3 oldPosition)
        {
            base.OnControlPointMoved(controlPoint, oldPosition);
            
            if (ReferenceEquals(controlPoint, lerpPoint))
                HandleAnchorLerpPointMovement();
        }

        private void HandleAnchorLerpPointMovement()
        {
            OnChanged();
        }
        
        public override void ApplyConstrain()
        {
            anchorPointA.ApplyConstrain();
            anchorPointB.ApplyConstrain();
            lerpPoint.ApplyConstrain();
        }

        public override float EstimateLength()
        {
            var netLengthOfControlPoints = Vector3.Distance(anchorPointA.Position, lerpPoint.Position) +
                                           Vector3.Distance(lerpPoint.Position, anchorPointB.Position);

            return Vector3.Distance(anchorPointA.Position, anchorPointB.Position) + netLengthOfControlPoints / 2;
        }

        public override Vector3 Evaluate(float t)
        {
            return Bezier.EvaluateQuadratic(anchorPointA.Position, anchorPointB.Position, lerpPoint.Position, t);
        }

        public override void SetPlane(Plane newPlane)
        {
            anchorPointA.SetPlane(newPlane);
            anchorPointB.SetPlane(newPlane);
            lerpPoint.SetPlane(newPlane);
            
            ApplyConstrain();
        }
        
        public override bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, anchorPointA) || 
                   ReferenceEquals(controlPoint, anchorPointB) ||
                   ReferenceEquals(controlPoint, lerpPoint);
        }
    }
}