using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class LerpPointConnector : Connector
    {
        [SerializeReference] 
        private ControlPoint anchorPoint;

        [SerializeField] [HideInInspector]
        public bool isBroken;

        public ControlPoint AnchorPoint => anchorPoint;
        public bool IsBroken => isBroken;

        public LerpPointConnector(ControlPoint anchorPoint, ControlPoint lerpPointA, ControlPoint lerpPointB) : base(lerpPointA, lerpPointB)
        {
            this.anchorPoint = anchorPoint;

            controlPointB.Position = anchorPoint.Position + (anchorPoint.Position - controlPointA.Position);
        }

        public void ToggleBroken()
        {
            if (isBroken)
                controlPointB.Position = anchorPoint.Position + (anchorPoint.Position - controlPointA.Position);

            isBroken = !isBroken;
        }

        public override void ReactToMovedPoint(ControlPoint movedPoint, Vector3 oldPosition)
        {
            if (isBroken)
                return;
            
            var reactPosition = anchorPoint.Position + (anchorPoint.Position - movedPoint.Position);
            
            if (ReferenceEquals(movedPoint, controlPointA))
                controlPointB.Position = reactPosition;
            
            if (ReferenceEquals(movedPoint, controlPointB))
                controlPointA.Position = reactPosition;
        }
        
        public override List<ControlPoint> ConnectedControlPoints
        {
            get => new List<ControlPoint>() { controlPointA, controlPointB, anchorPoint };
        }
        
        public override bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, controlPointA) ||
                   ReferenceEquals(controlPoint, controlPointB) ||
                   ReferenceEquals(controlPoint, anchorPoint);
        }
    }
}