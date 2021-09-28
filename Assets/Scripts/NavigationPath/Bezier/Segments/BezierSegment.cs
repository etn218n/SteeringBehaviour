using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public abstract class BezierSegment
    {
        [SerializeReference] protected BezierPath ownerPath;
        [SerializeReference] protected BezierSegment nextSegment;
        [SerializeReference] protected BezierSegment previousSegment;
        [SerializeReference] protected ControlPoint anchorPointA;
        [SerializeReference] protected ControlPoint anchorPointB;

        public BezierSegment NextSegment
        {
            get => nextSegment;
            set
            {
                if (value == this)
                    return;

                nextSegment = value;
                OnNextSegmentConnected(nextSegment);
            }
        }
        
        public BezierSegment PreviousSegment
        {
            get => previousSegment;
            set
            {
                if (value == this)
                    return;

                previousSegment = value;
                OnPreviousSegmentConnected(previousSegment);
            }
        }
        
        public ControlPoint AnchorPointA => anchorPointA;
        public ControlPoint AnchorPointB => anchorPointB;

        protected BezierSegment(Vector3 anchorPointA, Vector3 anchorPointB, BezierPath ownerPath)
        {
            this.ownerPath = ownerPath;
            
            this.anchorPointA = new ControlPoint(anchorPointA, this);
            this.anchorPointB = new ControlPoint(anchorPointB, this);
        }

        public void OnChanged()
        {
            ownerPath.RegeneratePoints();
        }

        public void ConnectPreviousSegment(BezierSegment segment)
        {
            if (segment == this)
                return;

            previousSegment = segment;
        }

        public virtual void OnControlPointMoved(ControlPoint controlPoint, Vector3 oldPosition)
        {
            if (ReferenceEquals(controlPoint, anchorPointA))
                HandleAnchorPointAMovement(oldPosition);
            else if (ReferenceEquals(controlPoint, anchorPointB))
                HandleAnchorPointBMovement(oldPosition);
        }

        protected virtual void HandleAnchorPointAMovement(Vector3 oldPosition)
        {
            if (previousSegment != null)
            {
                previousSegment.AnchorPointB.ReactToPairControlPoint(anchorPointA.Position);
                previousSegment.OnChanged();
            }
            
            OnChanged();
        }

        protected virtual void HandleAnchorPointBMovement(Vector3 oldPosition)
        {
            if (nextSegment != null)
            {
                nextSegment.AnchorPointA.ReactToPairControlPoint(anchorPointB.Position);
                nextSegment.OnChanged();
            }
            
            OnChanged();
        }

        public virtual void SetPlane(Plane newPlane)
        {
            anchorPointA.SetPlane(newPlane);
            anchorPointB.SetPlane(newPlane);
        }
        
        public virtual void ApplyConstrain()
        {
            anchorPointA.ApplyConstrain();
            anchorPointB.ApplyConstrain();
        }
        
        protected virtual void OnNextSegmentConnected(BezierSegment connectedSegment) { }
        protected virtual void OnPreviousSegmentConnected(BezierSegment connectedSegment) { }

        public abstract float EstimateLength();
        public abstract Vector3 Evaluate(float t);
        public abstract List<ControlPoint> ControlPoints { get; }

        public virtual bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, anchorPointA) || ReferenceEquals(controlPoint, anchorPointB);
        }
    }
}
