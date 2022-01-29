using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using BestVoxels.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 5f;

        [Header("Patrol")]
        [SerializeField] private PatrolPath _patrolPath;
        [Range(0f,1f)]
        [SerializeField] private float _patrolSpeedFraction = 0.3f;
        [SerializeField] private float _waypointDwellTime = 2f;
        [Tooltip("The Smaller number to Closer it will walk to the waypoint")]
        [SerializeField] private float _waypointReachDistance = 1f;

        [Header("Guard")]
        [Tooltip("The Smaller number to Closer it will walk to the waypoint")]
        [SerializeField] private float _guardReachDistance = 0.2f;
        [SerializeField] private float _guardRotateSpeed = 10f;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Transform _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;

        private AutoInit<Vector3> _guardPosition;
        private AutoInit<Quaternion> _guardRotation;

        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _player = GameObject.FindWithTag("Player").transform;
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();

            _guardPosition = new AutoInit<Vector3>(GetInitialGuardPosition);
            _guardRotation = new AutoInit<Quaternion>(GetInitialGuardRotation);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
            _guardRotation.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead) return;

            if (IsInChaseRange() && _fighter.CanAttack(_player.gameObject))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0f;
            _fighter.Attack(_player.gameObject);
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.StopCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;

            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, _patrolSpeedFraction);

                if (AtGuardPosition())
                {
                    Utilities.SmoothRotateTo(transform, _guardRotation.value, _guardRotateSpeed);
                }
            }
        }

        private bool AtWaypoint() => Vector3.Distance(transform.position, GetCurrentWaypoint()) < _waypointReachDistance;

        private void CycleWaypoint() => _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);

        private Vector3 GetCurrentWaypoint() => _patrolPath.GetWaypoint(_currentWaypointIndex);

        private bool AtGuardPosition() => Vector3.Distance(transform.position, _guardPosition.value) < _guardReachDistance && _patrolPath == null;


        private bool IsInChaseRange() => Vector3.Distance(transform.position, _player.position) < _chaseDistance;
        #endregion



        #region --Methods-- (Subscriber)
        private Vector3 GetInitialGuardPosition() => transform.position;

        private Quaternion GetInitialGuardRotation() => transform.rotation;
        #endregion
    }
}