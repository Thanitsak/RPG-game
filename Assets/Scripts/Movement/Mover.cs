using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Mover : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Fields-- (In Class)
        private NavMeshAgent _agent;
        private Animator _animator;
        private Fighter _fighter;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _fighter = GetComponent<Fighter>();
        }

        private void Update()
        {
            AnimateCharacter();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartMoveAction(Vector3 destination)
        {
            _fighter.CancelAttack();

            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.SetDestination(destination);
            _agent.isStopped = false;
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
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