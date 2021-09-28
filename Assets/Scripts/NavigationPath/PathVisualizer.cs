using UnityEngine;

namespace NavigationPath
{
    [RequireComponent(typeof(LineRenderer))]
    public class PathVisualizer : MonoBehaviour
    {
        [SerializeField] private Path path;
        [SerializeField] private bool showPoints;
        [SerializeField] private float pointSize;
        
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();

            Draw();
            path.OnChanged += Draw;
        }

        private void OnDrawGizmosSelected()
        {
            if (showPoints && path.Points.Count != 0)
            {
                Gizmos.color = Color.red;
                
                foreach (var point in path.Points)
                    Gizmos.DrawSphere(point, pointSize);
            }
        }

        public void Draw()
        {
            var points = path.Points;
            
            lineRenderer.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
                lineRenderer.SetPosition(i, points[i]);
        }
    }
}