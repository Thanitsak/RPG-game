using UnityEngine;
using UnityEngine.AI;

namespace RPGgame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Camera _camera;
        #endregion



        #region --Fields-- (In Class)
        private NavMeshAgent _agent;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoveToCursorClickPosition();
            }
        }
        #endregion



        #region --Methods-- (Custome PRIVATE)
        private void MoveToCursorClickPosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // get ray direction from camera to a screen point

            // Draw the ray
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                _agent.SetDestination(hitInfo.point);
            }
        }
        #endregion
    }
}