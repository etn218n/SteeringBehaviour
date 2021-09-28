using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class CubicAnchorPointConnector : Connector
    {
        [SerializeReference] 
        private ControlPoint lerpPointA;

        [SerializeReference] 
        private ControlPoint lerpPointB;

        public CubicAnchorPointConnector(ControlPoint anchorPointA, ControlPoint anchorPointB, ControlPoint lerpPointA, ControlPoint lerpPointB)
            : base(anchorPointA, anchorPointB)
        {
            this.lerpPointA = lerpPointA;
            this.lerpPointB = lerpPointB;
        }

        public override void ReactToMovedPoint(ControlPoint movedPoint, Vector3 oldPosition)
        {
            controlPointA.Position = movedPoint.Position;
            controlPointB.Position = movedPoint.Position;

            var displacement = movedPoint.Position - oldPosition;

            lerpPointA.Position += displacement;
            lerpPointB.Position += displacement;
        }

        public override List<ControlPoint> ConnectedControlPoints
        {
            get => new List<ControlPoint>() { controlPointA, controlPointB, lerpPointA, lerpPointB };
        }

        public override bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, controlPointA) ||
                   ReferenceEquals(controlPoint, controlPointB);
        }
    }
}