using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _maxSpeed = 5.66f;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private NavMeshAgent _agent;
        private Animator _animator;
        private Health _health;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();

            if (!_agent.enabled)
                _agent.enabled = true;
        }

        private void Update()
        {
            if (_health.IsDead && _agent.enabled)
                _agent.enabled = false;

            AnimateCharacter();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);

            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.SetDestination(destination);
            _agent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _agent.isStopped = false;
        }

        public void CancelMoveTo()
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



        #region --Methods-- (Animation Event)
        private void FootR()
        {
        }

        private void FootL()
        {
        }
        #endregion



        #region --Methods-- (Interface)
        void IAction.Cancel()
        {
            CancelMoveTo();
        }

        object ISaveable.CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            
            return data;
        }

        void ISaveable.RestoreState(object state) // When level loaded it get called AFTER Awake(), BEFORE Start()
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            GetComponent<NavMeshAgent>().Warp(((SerializableVector3)stateDict["position"]).ToVector()); // Using Warp() it also Cancel Old Target Postition that it Need to Move To
            transform.eulerAngles = ((SerializableVector3)stateDict["rotation"]).ToVector();
        }
        #endregion
    }
}