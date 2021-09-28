using System;
using UnityEngine;

namespace NavigationPath
{
    [Serializable]
    public struct Plane
    {
        [SerializeField] private Vector3 origin;
        [SerializeField] private Vector3 normal;
        [SerializeField] private Vector3 offset;
        public Vector3 Origin => origin;
        public Vector3 Normal => normal;
        
        public Vector3 Constrain(Vector3 point)
        {
            var v = point - origin;
            var d = Vector3.Project(v, normal.normalized);
            
            return point - d + offset;
        }
    }
}
