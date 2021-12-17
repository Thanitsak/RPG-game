using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 5f;
        #endregion



        #region --Fields-- (In Class)
        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;

        private ActionScheduler _actionScheduler;

        private Transform _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _guardPosition = transform.position;

            _actionScheduler = GetComponent<ActionScheduler>();

            _player = GameObject.FindWithTag("Player").transform;
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (_health.IsDead) return;

            if (IsInChaseRange() && _fighter.CanAttack(_player.gameObject))
            {
                _timeSinceLastSawPlayer = 0f;
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            _timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AttackBehaviour()
        {
            _fighter.Attack(_player.gameObject);
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.StopCurrentAction();
        }

        private void GuardBehaviour()
        {
            _mover.StartMoveAction(_guardPosition);
        }

        private bool IsInChaseRange() => Vector3.Distance(transform.position, _player.position) < _chaseDistance;
        #endregion
    }
}