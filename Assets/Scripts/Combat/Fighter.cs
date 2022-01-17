using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _chaseSpeedFraction = 1f;
        [SerializeField] private float _rotateSpeed = 10f;
        [Header("Weapon")]
        [SerializeField] private Transform _handTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Health _target;
        private Mover _mover;
        private Animator _animator;

        private float _timeSinceLastAttack = Mathf.Infinity;

        private Weapon _currentWeapon = null;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();

            EquippedWeapon(_defaultWeapon);
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null) return;
            if (_target.IsDead) return;

            if (!IsInStopRange())
            {
                _mover.MoveTo(_target.transform.position, _chaseSpeedFraction);
            }
            else
            {
                _mover.CancelMoveTo();
                AttackBehaviour();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void EquippedWeapon(Weapon equippedWeapon)
        {
            _currentWeapon = equippedWeapon;
            equippedWeapon.Spawn(_handTransform, _animator);
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);

            _target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            Health target = combatTarget.GetComponent<Health>();
            return target != null && !target.IsDead;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CancelAttack()
        {
            _target = null;

            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("StopAttack");

            _mover.CancelMoveTo();
        }

        private void AttackBehaviour()
        {
            Utilities.SmoothRotateTo(transform, _target.transform, _rotateSpeed);

            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("StopAttack"); // Reset first so no weird movement when player gonna attack
            _animator.SetTrigger("Attack"); // This will Trigger the Hit() event
        }

        private bool IsInStopRange() => Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.Range;

        // For Animation Event
        private void Hit()
        {
            if (_target == null) return;

            _target.TakeDamage(_currentWeapon.Damage);
        }
        #endregion



        #region --Methods-- (Interface)
        void IAction.Cancel()
        {
            CancelAttack();
        }
        #endregion
    }
}