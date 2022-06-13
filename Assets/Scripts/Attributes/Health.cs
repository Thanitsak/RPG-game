using System;
using UnityEngine.Events;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Utils;
using RPG.Control;

namespace RPG.Attributes
{
    /// <summary>
    /// BEWARE this script will search through from the root to all the children for Respawner script
    /// so for Player it's fine cuz only one script under the root, but for Enemies many scripts are under the root.
    /// </summary>
    public class Health : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [Tooltip("How much health will restore to player in percentage")]
        [Range(1f, 100f)]
        [SerializeField] private float _healthRegeneratePercentage = 100f;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("UnityEvent")]
        [SerializeField] private UnityEvent<float> _onTakeDamage;
        [SerializeField] private UnityEvent _onDie;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnHealthChanged;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Animator _animator;
        private BaseStats _baseStats;
        private Respawner _respawner;

        private bool _wasDeadLastFrame = false;
        #endregion



        #region --Properties-- (Auto)
        public AutoInit<float> HealthPoints { get; private set; }
        #endregion



        #region --Properties-- (With Backing Fields)
        public float MaxHealthPoints { get => _baseStats.GetHealth(); }
        public bool IsDead { get => HealthPoints.value <= 0f; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _respawner = transform.root.GetComponentInChildren<Respawner>();

            HealthPoints = new AutoInit<float>(GetInitialHealth);
        }

        private void OnEnable()
        {
            _baseStats.OnLevelUpSetup += () => RegenerateHealth(_healthRegeneratePercentage); // see at Action declaration why this Action
        }

        private void Start()
        {
            HealthPoints.ForceInit();
        }

        private void OnDisable()
        {
            _baseStats.OnLevelUpSetup -= () => RegenerateHealth(_healthRegeneratePercentage);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Deal Damage to the GameObject, and also Give XP to attacker
        /// </summary>
        /// <param name="attacker">attacker GameObject ex. 'player'</param>
        /// <param name="damage">ONLY positive value are expected to deal damage, to heal use different method.</param>
        public void TakeDamage(GameObject attacker, float damage)
        {
            if (IsDead) return;

            HealthPoints.value = Mathf.Max(0f, HealthPoints.value - damage);
            if (IsDead)
            {
                AwardExperience(attacker);
                _onDie?.Invoke();
            }
            else
            {
                _onTakeDamage?.Invoke(damage);
            }

            UpdateState();
            OnHealthChanged?.Invoke();
        }

        /// <summary>
        /// Heal the GameObject
        /// </summary>
        /// <param name="healAmount">ONLY positive value are expected to heal, to do damage use different method.</param>
        public void Heal(float healAmount)
        {
            HealthPoints.value = Mathf.Clamp(HealthPoints.value + healAmount, 0f, MaxHealthPoints);

            UpdateState();
            OnHealthChanged?.Invoke();
        }

        public float GetPercentage()
        {
            return GetPercentageDecimal() * 100f;
        }

        public float GetPercentageDecimal()
        {
            return Mathf.InverseLerp(0f, MaxHealthPoints, HealthPoints.value);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Subscriber~
        public void RegenerateHealth(float regeneratePercentage)
        {
            float regenHealthPoints = (MaxHealthPoints * regeneratePercentage) / 100f;
            HealthPoints.value = Mathf.Max(HealthPoints.value, regenHealthPoints);

            UpdateState();
            OnHealthChanged?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AwardExperience(GameObject attacker)
        {
            Experience experience = attacker.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(_baseStats.GetExperienceReward());
        }

        private void UpdateState()
        {
            if (!_wasDeadLastFrame && IsDead) // NavMeshAgent Get disabled in Mover class | Can walk through because Is Kinematic need to be turn on
            {
                _animator.SetTrigger("Die");
                _actionScheduler.StopCurrentAction();
            }
            if (_wasDeadLastFrame && !IsDead) // NavMeshAgent Get Enabled in Mover class so that player can walk on NavMesh again without error
            {
                _animator.Rebind();
            }

            _wasDeadLastFrame = IsDead;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private float GetInitialHealth() => MaxHealthPoints;
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return HealthPoints.value;
        }

        void ISaveable.RestoreState(object state)
        {
            HealthPoints.value = (float)state;

            // Respawn only for Player when save & exit while player is dead
            if (_respawner != null && IsDead) // Check for not null so that Enemy won't trigger this (no enemy has Respawner component)
                _respawner.Respawn();

            UpdateState();
            OnHealthChanged?.Invoke();
        }
        #endregion
    }
}