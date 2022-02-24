using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _chaseSpeedFraction = 1f;
        [SerializeField] private float _rotateSpeed = 10f;
        [Header("Weapon")]
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [Tooltip("Default Weapon will be used when nothing is being equipped in the Weapon Slot")]
        [SerializeField] private WeaponConfig _defaultWeapon = null;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Health _target;
        private Mover _mover;
        private Animator _animator;
        private BaseStats _baseStats;

        private float _timeSinceLastAttack = Mathf.Infinity;

        private WeaponConfig _currentWeaponConfig;
        private AutoInit<Weapon> _currentWeapon;
        private Equipment _equipment;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _equipment = GetComponent<Equipment>();

            _currentWeapon = new AutoInit<Weapon>(GetInitialCurrentWeapon);
            _currentWeaponConfig = _defaultWeapon;
        }

        private void OnEnable()
        {
            if (_equipment != null)
                _equipment.equipmentUpdated += UpdateWeapon;
        }

        private void Start()
        {
            _currentWeapon.ForceInit(); // Init Default Weapon, Also WON'T Run if already init from save (but it will since save run after this)
        }

        private void OnDisable()
        {
            if (_equipment != null)
                _equipment.equipmentUpdated -= UpdateWeapon;
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null) return;
            if (_target.IsDead) return;

            if (!IsInStopRange(_target.transform))
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
        public void EquippedWeapon(WeaponConfig pickupWeapon)
        {
            _currentWeaponConfig = pickupWeapon;
            _currentWeapon.value = AttachWeaopn(pickupWeapon);
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);

            _target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            if (!_mover.CanMoveToNoMaxLength(combatTarget.transform.position) && !IsInStopRange(combatTarget.transform))
                return false;

            Health target = combatTarget.GetComponent<Health>();
            return target != null && !target.IsDead;
        }

        public Health GetTarget()
        {
            return _target;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private Weapon AttachWeaopn(WeaponConfig weapon)
        {
            return weapon.Spawn(_rightHandTransform, _leftHandTransform, _animator);
        }

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

        private bool IsInStopRange(Transform targetTransform) => Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.Range;
        #endregion



        #region --Methods-- (Animation Event)
        private void Hit()
        {
            if (_target == null) return;

            float damage = _baseStats.GetDamage();

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }

            if (_currentWeaponConfig.HasProjectile)
            {
                _currentWeaponConfig.LaunchProjectile(gameObject, _target, damage);
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



        #region --Methods-- (Subscriber)
        private void UpdateWeapon()
        {
            WeaponConfig itemInWeaponSlot = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            if (itemInWeaponSlot != null)
            {
                EquippedWeapon(itemInWeaponSlot);
            }
            else
            {
                EquippedWeapon(_defaultWeapon);
            }
        }

        private Weapon GetInitialCurrentWeapon()
        {
            return AttachWeaopn(_defaultWeapon);
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
            return _currentWeaponConfig.name; // name of the file
        }

        void ISaveable.RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquippedWeapon(weapon);
        }
        #endregion
    }
}