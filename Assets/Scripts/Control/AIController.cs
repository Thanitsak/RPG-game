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
        [SerializeField] private float _aggravateCoolDownTime = 8f;
        [SerializeField] private float _shoutDistance = 6f;

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
        private float _timeSinceAggravated = Mathf.Infinity;
        private AIController _aggravateCaller = null;
        private bool _canAggravate = true;
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

            if (IsAggravated() && _fighter.CanAttack(_player.gameObject))
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



        #region --Methods-- (Custom PUBLIC)
        public void Aggravate(AIController caller)
        {
            _timeSinceAggravated = 0f;
            _aggravateCaller = caller;
            _canAggravate = true;
            print(caller);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggravated += Time.deltaTime;

            if (_timeSinceAggravated >= _aggravateCoolDownTime)
                _canAggravate = true;
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0f;
            _fighter.Attack(_player.gameObject);

            if (_canAggravate)
            {
                AggravateNearbyEnemies();
                _canAggravate = false;
            }
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

        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            return distanceToPlayer < _chaseDistance || _timeSinceAggravated < _aggravateCoolDownTime;
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _shoutDistance, Vector3.up, 0f);

            print(transform.name + " STARTED");
            print(transform.name + hits.Length);
            foreach (RaycastHit eachHit in hits)
            {
                
                AIController otherAI = eachHit.transform.GetComponent<AIController>();
                if (otherAI == null || otherAI == _aggravateCaller || otherAI == this) continue;
                print(transform.name + " Activate : " + eachHit.transform.name);
                otherAI.Aggravate(this);
            }
            print(transform.name + " ENDED");
        }

        private bool AtWaypoint() => Vector3.Distance(transform.position, GetCurrentWaypoint()) < _waypointReachDistance;

        private void CycleWaypoint() => _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex); // SAVE CurrentWaypointIndex for enemy to continue follow

        private Vector3 GetCurrentWaypoint() => _patrolPath.GetWaypoint(_currentWaypointIndex);

        private bool AtGuardPosition() => Vector3.Distance(transform.position, _guardPosition.value) < _guardReachDistance && _patrolPath == null;
        #endregion



        #region --Methods-- (Subscriber)
        private Vector3 GetInitialGuardPosition() => transform.position;

        private Quaternion GetInitialGuardRotation() => transform.rotation;
        #endregion
    }
}