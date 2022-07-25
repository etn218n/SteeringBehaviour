using UnityEngine;

namespace Steering
{
    public class AssignTarget : MonoBehaviour
    {
        [SerializeField] private Transform targetToMove;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;

                targetToMove.position = mouseWorldPosition;
            }
        }
    }
}
