﻿using UnityEngine;

namespace Kinematics
{
    public struct Kinematic
    {
        public static readonly Kinematic Empty = new Kinematic();
        
        public Vector3 Up;
        public Vector3 Forward;
        public Vector3 Position;
        public Quaternion Orientation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
    }
}