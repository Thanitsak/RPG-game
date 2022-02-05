using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using BestVoxels.Utils;
using UnityEngine.Events;
using System;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [Range(1f, 100f)]
        [SerializeField] private float _healthRegneratePercentage = 70f;
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
        #endregion



        #region --Properties-- (Auto)
        public AutoInit<float> HealthPoints { get; private set; }
        public bool IsDead { get; private set; } = false;
        #endregion



        #region --Properties-- (With Backing Fields)
        public float MaxHealthPoints { get { return _baseStats.GetHealth(); } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();

            HealthPoints = new AutoInit<float>(GetInitialHealth);
        }

        private void OnEnable()
        {
            _baseStats.OnLevelUp += RegenerateHealth;
        }

        private void Start()
        {
            HealthPoints.ForceInit();
        }

        private void OnDisable()
        {
            _baseStats.OnLevelUp -= RegenerateHealth;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void TakeDamage(GameObject attacker, float damage)
        {
            HealthPoints.value = Mathf.Max(0f, HealthPoints.value - damage);

            if (HealthPoints.value <= 0f)
            {
                _onDie?.Invoke();

                DeathBehaviour();

                AwardExperience(attacker);
            }

            _onTakeDamage?.Invoke(damage);
            OnHealthChanged?.Invoke();
        }

        public float GetPercentage()
        {
            return GetPercentageDecimal() * 100f;
        }

        public float GetPercentageDecimal()
        {
            return Mathf.InverseLerp(0f, _baseStats.GetHealth(), HealthPoints.value);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AwardExperience(GameObject attacker)
        {
            Experience experience = attacker.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(_baseStats.GetExperienceReward());
        }

        private void DeathBehaviour()
        {
            if (IsDead) return;

            IsDead = true;

            _animator.SetTrigger("Die");
            _actionScheduler.StopCurrentAction();
            // NavMeshAgent Get disabled in Mover class | Can walk through because Is Kinematic need to be turn on
        }
        #endregion



        #region --Methods-- (Subscriber)
        private float GetInitialHealth() => _baseStats.GetHealth();

        private void RegenerateHealth()
        {
            float regenHealthPoints = (_baseStats.GetHealth() * _healthRegneratePercentage) / 100f;
            HealthPoints.value = Mathf.Max(HealthPoints.value, regenHealthPoints);

            OnHealthChanged?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return HealthPoints.value;
        }

        void ISaveable.RestoreState(object state) // When level loaded it get called AFTER Awake(), BEFORE Start()
        {
            HealthPoints.value = (float)state;
            
            if (HealthPoints.value <= 0f)
            {
                DeathBehaviour();
            }

            OnHealthChanged?.Invoke();
        }
        #endregion
    }
}