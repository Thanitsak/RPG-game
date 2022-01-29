using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _chaseSpeedFraction = 1f;
        [SerializeField] private float _rotateSpeed = 10f;
        [Header("Weapon")]
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Health _target;
        private Mover _mover;
        private Animator _animator;
        private BaseStats _baseStats;

        private float _timeSinceLastAttack = Mathf.Infinity;

        private Weapon _currentWeapon = null;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (_currentWeapon == null) // Just to make sure this won't override Load Data but it won't anyway cuz in SavingSystem already wait for 1 frame then load
            {
                EquippedWeapon(_defaultWeapon);
            }
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
            equippedWeapon.Spawn(_rightHandTransform, _leftHandTransform, _animator);
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

        public Health GetTarget()
        {
            return _target;
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
        #endregion



        #region --Methods-- (Animation Event)
        private void Hit()
        {
            if (_target == null) return;

            float damage = _baseStats.GetDamage();

            if (_currentWeapon.HasProjectile)
            {
                _currentWeapon.LaunchProjectile(gameObject, _target, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot()
        {
            Hit();
        }
        #endregion



        #region --Methods-- (Interface)
        void IAction.Cancel()
        {
            CancelAttack();
        }

        // SAVING WILL Be better way on RPG part 2 course
        object ISaveable.CaptureState()
        {
            return _currentWeapon.name; // name of the file
        }

        void ISaveable.RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquippedWeapon(weapon);
        }

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatType statType)
        {
            if (statType == StatType.Damage)
            {
                yield return _currentWeapon.Damage;
                // This way it's concisely to say that we want to return something Otherwise return nothing or as empty list since we are using IEnumerable it's handy
                // We can also return more than one thing by doing 'yield return _anotherCurrenetWeapon.Damage;' as it's allow in IEnumerable
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatType statType)
        {
            if (statType == StatType.Damage)
            {
                yield return _currentWeapon.DamageBonusPercentage;
            }
        }
        #endregion
    }
}