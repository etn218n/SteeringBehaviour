using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class Connector
    {
        [SerializeReference] 
        protected ControlPoint controlPointA;
        
        [SerializeReference]
        protected ControlPoint controlPointB;
        
        public Connector(ControlPoint controlPointA, ControlPoint controlPointB)
        {
            this.controlPointA = controlPointA;
            this.controlPointB = controlPointB;
        }

        public virtual void ReactToMovedPoint(ControlPoint movedPoint, Vector3 oldPosition) { }

        public virtual bool Contains(ControlPoint controlPoint)
        {
            return ReferenceEquals(controlPoint, controlPointA) || ReferenceEquals(controlPoint, controlPointB);
        }

        public virtual List<ControlPoint> ConnectedControlPoints
        {
            get => new List<ControlPoint>() { controlPointA, controlPointB };
        }
    }
}