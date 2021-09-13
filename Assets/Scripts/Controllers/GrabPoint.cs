using UnityEngine;

namespace Controllers
{
    public class GrabPoint
    {
        public bool IsGrabbed { get; private set; }
        public Vector3 Position { get; private set; }
        

        public void Grab(Vector3 grabPoint)
        {
            IsGrabbed = true;
            Position  = grabPoint;
        }
        

        public void Ungrab()
        {
            IsGrabbed = false;
        }
    }
}