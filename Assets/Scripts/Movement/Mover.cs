using UnityEngine;
using UnityEngine.AI;

namespace RPGgame
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Mover : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Fields-- (In Class)
        private Camera _camera;
        private NavMeshAgent _agent;
        private Animator _animator;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _camera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursorClickPosition();
            }

            AnimateCharacter();
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

        private void AnimateCharacter()
        {
            Vector3 globalVelocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity); // Convert Velocity from Global into Local, velocity relative to player point of view. (this case localVelocity & globalVelocity return same magnitude)

            float forwardSpeed = localVelocity.z; // We could use .magnitude BUT it just speed in all direction, also count when move left, right. LOOK MORE NATURAL with .z which only takes forward speed

            _animator.SetFloat("MoveSpeed", forwardSpeed);
        }
        #endregion
    }
}