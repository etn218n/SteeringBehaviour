using System;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class AnchorPointConnector : Connector
    {
        public AnchorPointConnector(ControlPoint anchorPointA, ControlPoint anchorPointB) : base(anchorPointA, anchorPointB) { }

        public override void ReactToMovedPoint(ControlPoint movedPoint, Vector3 oldPosition)
        {
            controlPointA.Position = movedPoint.Position;
            controlPointB.Position = movedPoint.Position;
        }
    }
}