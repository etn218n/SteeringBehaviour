using System;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public class ControlPoint
    {
        [SerializeField] private Plane plane;
        [SerializeField] private Vector3 position;
        
        [SerializeReference] private BezierSegment ownerSegment;
        
        public Vector3 Position
        {
            get => position;
            set
            {
                var oldPosition = position;
                position = plane.Constrain(value);
                ownerSegment.OnControlPointMoved(this, oldPosition);
            }
        }
        
        public ControlPoint(Vector3 position, BezierSegment ownerSegment)
        {
            this.position = position;
            this.ownerSegment = ownerSegment;
        }

        public void ApplyConstrain()
        {
            position = plane.Constrain(position);
        }

        public void ReactToPairControlPoint(Vector3 newPosition)
        {
            position = plane.Constrain(newPosition);
        }

        public void SetPlane(Plane newPlane)
        {
            plane = newPlane;
        }
    }

    
}