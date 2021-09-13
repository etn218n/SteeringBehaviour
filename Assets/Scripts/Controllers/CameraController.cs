using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private Camera mainCamera;
        private GrabPoint grabPoint;

        
        private void Awake()
        {
            grabPoint  = new GrabPoint();
            mainCamera = GetComponent<Camera>();
        }
        

        private void Update()
        {
            PanCamera();
            ZoomCamera();
        }

        
        private void PanCamera()
        {
            if (Input.GetMouseButtonDown(2))
            {
                grabPoint.Grab(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButtonUp(2))
            {
                grabPoint.Ungrab();
            }

            if (grabPoint.IsGrabbed)
            {
                var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var moveDirection = grabPoint.Position - mousePosition;

                mainCamera.transform.Translate(moveDirection);
            }
        }


        private void ZoomCamera()
        {
            var zoomLevel = mainCamera.orthographicSize - Input.mouseScrollDelta.y;
            
            mainCamera.orthographicSize = Mathf.Clamp(zoomLevel, 0.1f, 20f);
        }
    }
}
