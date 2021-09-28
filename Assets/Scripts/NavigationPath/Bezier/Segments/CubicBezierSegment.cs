using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class CubicBezierSegment : BezierSegment
    {
        [SerializeReference] private ControlPoint lerpPointA;
        [SerializeReference] private ControlPoint lerpPointB;

        [SerializeField] private bool isBrokenA;
        [SerializeField] private bool isBrokenB;
        
        public ControlPoint LerpPointA => lerpPointA;
        public ControlPoint LerpPointB => lerpPointB;

        public bool IsBrokenA
        {
            get => isBrokenA;
            set
            {
                isBrokenA = value;

                if (previousSegment is CubicBezierSegment connectedSegment)
                {
                    connectedSegment.isBrokenB = isBrokenA;

                    if (value == false) // sync their positions
                        HandleLerpPointAMovement(Vector3.zero);
                }
            } 
        }
        
        public bool IsBrokenB
        {
            get => isBrokenB;
            set
            {
                isBrokenB = value;

                if (nextSegment is CubicBezierSegment connectedSegment)
                {
                    connectedSegment.isBrokenA = isBrokenB;

                    if (value == false) // sync their positions
                        HandleLerpPointBMovement(Vector3.zero);
                }
            } 
        }
        
        public override List<ControlPoint> ControlPoints => new List<ControlPoint>(4) { anchorPointA, anchorPointB, lerpPointA, lerpPointB };
        
        public CubicBezierSegment(Vector3 anchorPointA, Vector3 anchorPointB, Vector3 lerpPointA, Vector3 lerpPointB, BezierPath ownerPath) 
            : base(anchorPointA, anchorPointB, ownerPath)
        {
            this.lerpPointA = new ControlPoint(lerpPointA, this);
            this.lerpPointB = new ControlPoint(lerpPointB, this);
        }

        public override void OnControlPointMoved(ControlPoint controlPoint, Vector3 oldPosition)
        {
            if (ReferenceEquals(controlPoint, anchorPointA))
                HandleAnchorPointAMovement(oldPosition);
            else if (ReferenceEquals(controlPoint, anchorPointB))
                HandleAnchorPointBMovement(oldPosition);
            else if (ReferenceEquals(controlPoint, lerpPointA))
                HandleLerpPointAMovement(oldPosition);
            else if (ReferenceEquals(controlPoint, lerpPointB))
                HandleLerpPointBMovement(oldPosition);
        }

        private void HandleLerpPointAMovement(Vector3 oldPosition)
        {
            if (previousSegment is CubicBezierSegment connectedSegment)
            {
                if (!isBrokenA && !connectedSegment.isBrokenB)
                {
                    var v = anchorPointA.Position - lerpPointA.Position;
                    connectedSegment.LerpPointB.ReactToPairControlPoint(anchorPointA.Position + v);
                    connectedSegment.OnChanged();
                }
            }

            OnChanged();
        }
        
        private void HandleLerpPointBMovement(Vector3 oldPosition)
        {
            if (nextSegment is CubicBezierSegment connectedSegment)
            {
                if (!isBrokenB && !connectedSegment.isBrokenA)
                {
                    var v = anchorPointB.Position - lerpPointB.Position;
                    connectedSegment.LerpPointA.ReactToPairControlPoint(anchorPointB.Position + v);
                    connectedSegment.OnChanged();
                }
            }

            OnChanged();
        }

        protected override void HandleAnchorPointAMovement(Vector3 oldPosition)
        {
            if (previousSegment is CubicBezierSegment connectedSegment)
            {
                var displacement = anchorPointA.Position - oldPosition;
                lerpPointA.ReactToPairControlPoint(lerpPointA.Position + displacement);
                
                connectedSegment.AnchorPointB.ReactToPairControlPoint(anchorPointA.Position);
                connectedSegment.LerpPointB.ReactToPairControlPoint(connectedSegment.LerpPointB.Position + displacement);
                connectedSegment.OnChanged();
            }
            else if (previousSegment != null)
            {
                previousSegment.AnchorPointB.ReactToPairControlPoint(anchorPointA.Position);
                previousSegment.OnChanged();
            }
            
            OnChanged();
        }
        
        protected override void HandleAnchorPointBMovement(Vector3 oldPosition)
        {
            if (nextSegment is CubicBezierSegment connectedSegment)
            {
                var displacement = anchorPointB.Position - oldPosition;
                lerpPointB.ReactToPairControlPoint(lerpPointB.Position + displacement);
                
                connectedSegment.AnchorPointA.ReactToPairControlPoint(anchorPointB.Position);
                connectedSegment.LerpPointA.ReactToPairControlPoint(connectedSegment.LerpPointA.Position + displacement);
                connectedSegment.OnChanged();
            }
            else if (nextSegment != null)
            {
                nextSegment.AnchorPointA.ReactToPairControlPoint(anchorPointB.Position);
                nextSegment.OnChanged();
            }

            OnChanged();
        }
        
        public override void ApplyConstrain()
        {
            anchorPointA.ApplyConstrain();
            anchorPointB.ApplyConstrain();
            lerpPointA.ApplyConstrain();
            lerpPointB.ApplyConstrain();
        }
        
        public override void SetPlane(Plane newPlane)
        {
            anchorPointA.SetPlane(newPlane);
            anchorPointB.SetPlane(newPlane);
            lerpPointA.SetPlane(newPlane);
            lerpPointB.SetPlane(newPlane);
            
            ApplyConstrain();
        }

        public override float EstimateLength()
        {
            var netLengthOfControlPoints = Vector3.Distance(anchorPointA.Position, lerpPointA.Position) +
                                           Vector3.Distance(lerpPointA.Position, lerpPointB.Position) +
                                           Vector3.Distance(lerpPointB.Position, anchorPointB.Position);

            return Vector3.Distance(anchorPointA.Position, anchorPointB.Position) + netLengthOfControlPoints / 2;
        }

        public override Vector3 Evaluate(float t)
        {
            return Bezier.EvaluateCubic(anchorPointA.Position, anchorPointB.Position, lerpPointA.Position, lerpPointB.Position, t);
        }

        protected override void OnNextSegmentConnected(BezierSegment connectedSegment)
        {
            if (connectedSegment is CubicBezierSegment segment)
            {
                this.isBrokenB = segment.isBrokenA;
                
                if (!isBrokenB)
                {
                    var v = anchorPointB.Position - lerpPointB.Position;
                    segment.LerpPointA.ReactToPairControlPoint(anchorPointB.Position + v);
                }
            }
        }

        protected override void OnPreviousSegmentConnected(BezierSegment connectedSegment)
        {
            if (connectedSegment is CubicBezierSegment segment)
            {
                this.isBrokenA = segment.isBrokenB;
                
                if (!isBrokenA)
                {
                    var v = anchorPointA.Position - lerpPointA.Position;
                    segment.LerpPointB.ReactToPairControlPoint(anchorPointA.Position + v);
                }
            }
        }

        public override bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, anchorPointA) || 
                   ReferenceEquals(controlPoint, anchorPointB) || 
                   ReferenceEquals(controlPoint, lerpPointA)   || 
                   ReferenceEquals(controlPoint, lerpPointB);
        }
    }
}