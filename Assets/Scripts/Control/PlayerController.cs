using UnityEngine;
using RPG.Movement;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Fields-- (In Class)
        private Camera _camera;
        private Mover _mover;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _camera = Camera.main;
            _mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }
        }
        #endregion



        #region --Methods-- (Custome PRIVATE)
        private void MoveToCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // get ray direction from camera to a screen point

            // Draw the ray
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                _mover.MoveTo(hitInfo.point);
            }
        }
        #endregion
    }
}