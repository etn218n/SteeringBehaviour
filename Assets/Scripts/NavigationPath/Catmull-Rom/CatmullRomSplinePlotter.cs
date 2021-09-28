using Kinematics;
using UnityEngine;

namespace NavigationPath
{
    public class CatmullRomSplinePlotter : MonoBehaviour
    {
        [Header("Camera Setting")] 
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private float projectionDistance;
        
        [Header("Components")]
        [SerializeField] private CatmullRomPath path;
        [SerializeField] private KinematicEntity kinematicEntity;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mouseScreenPosition = Input.mousePosition;
                mouseScreenPosition.z = projectionDistance;
                
                var mouseWorldPosition = controlledCamera.ScreenToWorldPoint(mouseScreenPosition);
                
                Plot(mouseWorldPosition);
            }
        }

        private void Plot(Vector3 point)
        {
            if (path.IsEmpty)
                path.CreateFirstSegment(kinematicEntity.CurrentKinematic.Position, point);
            else
                path.AddSegment(point);
        }
    }
}
