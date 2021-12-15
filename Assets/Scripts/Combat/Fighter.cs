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
        [SerializeField] private float _rotateSpeed = 10f;
        #endregion



        #region --Fields-- (In Class)
        private float _timeSinceLastAttack;

        private ActionScheduler _actionScheduler;

        private Health _target;
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
            if (_target.IsDead) return;

            if (!IsInStopRange())
            {
                _mover.MoveTo(_target.transform.position);
            }
            else
            {
                _mover.CancelMoveTo();
                AttackBehaviour();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Attack(CombatTarget combatTarget)
        {
            _actionScheduler.StartAction(this);

            _target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(CombatTarget combatTarget)
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
        }

        private void AttackBehaviour()
        {
            SmoothRotateTo(_target.transform);

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

        private void SmoothRotateTo(Transform target)
        {
            // Getting Direction from vector3 by using formula 'targetPos - ourPos'
            Vector3 direction = target.position - transform.position;

            // Get Rotation that we want to rotate to
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);

            // Gradually Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
        }

        private bool IsInStopRange() => Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;

        // For Animation Event
        private void Hit()
        {
            if (_target == null) return;

            _target.TakeDamage(_weaponDamage);
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