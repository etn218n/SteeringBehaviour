using UnityEngine;

namespace NavigationPath
{
    public static class Bezier
    {
        public static Vector3 EvaluateQuadratic(Vector3 anchorPointA, Vector3 anchorPointB, Vector3 lerpPoint, float t)
        {
            Vector3 p0 = Vector3.Lerp(anchorPointA, lerpPoint, t);
            Vector3 p1 = Vector3.Lerp(lerpPoint, anchorPointB, t);
            
            return Vector3.Lerp(p0, p1, t);
        }

        public static Vector3 EvaluateCubic(Vector3 anchorPointA, Vector3 anchorPointB, Vector3 lerpPointA, Vector3 lerpPointB, float t)
        {
            Vector3 p0 = EvaluateQuadratic(anchorPointA, lerpPointB, lerpPointA, t);
            Vector3 p1 = EvaluateQuadratic(lerpPointA, anchorPointB, lerpPointB, t);
            
            return Vector3.Lerp(p0, p1, t);
        }

        public static Vector3 EvaluateQuadraticTangent(Vector3 anchorPointA, Vector3 anchorPointB, Vector3 lerpPoint, float t)
        {
            return 2 * (1 - t) * (lerpPoint - anchorPointA) + 2 * t * (anchorPointB - lerpPoint);
        }
    }
}
