using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        #endregion



        #region --Fields-- (In Class)
        private float _timeSinceLastAttack;

        private ActionScheduler _actionScheduler;

        private Transform _target;
        private Mover _mover;
        private Animator _animator;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null) return;

            if (!IsInStopRange())
            {
                _mover.MoveTo(_target.position);
            }
            else
            {
                _mover.CancelMoveTo();
                AttackBehaviour();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Attack(CombatTarget target)
        {
            _actionScheduler.StartAction(this);

            _target = target.transform;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CancelAttack()
        {
            _target = null;
        }

        private void AttackBehaviour()
        {
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                _animator.SetTrigger("Attack"); // This will Trigger the Hit() event
                _timeSinceLastAttack = 0f;
            }
        }

        private bool IsInStopRange() => Vector3.Distance(transform.position, _target.position) < _weaponRange;

        // For Animation Event
        private void Hit()
        {
            if (_target == null) return;

            Health health = _target.GetComponent<Health>();
            health.TakeDamage(_weaponDamage);
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