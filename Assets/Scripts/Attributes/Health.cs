using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using BestVoxels.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [Range(1f, 100f)]
        [SerializeField] private float _healthRegneratePercentage = 70f;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Animator _animator;
        private BaseStats _baseStats;        
        #endregion



        #region --Properties-- (Auto)
        public bool IsDead { get; private set; } = false;
        public AutoInit<float> HealthPoints { get; private set; }
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
                DeathBehaviour();

                AwardExperience(attacker);
            }
        }

        public float GetPercentage()
        {
            // Add GetHealth() Differences l.1 vs l.2 diff is 30 then add 30 to healthPoints

            return Mathf.InverseLerp(0f, _baseStats.GetHealth(), HealthPoints.value) * 100f;
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
        }
        #endregion
    }
}