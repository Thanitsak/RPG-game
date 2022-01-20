using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _healthPoints = 100f;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Animator _animator;
        private BaseStats _baseStats;
        #endregion



        #region --Properties-- (Auto)
        public bool IsDead { get; private set; } = false;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();

            _healthPoints = _baseStats.GetHealth(); // If this run after Load Save then death enemy back to life BUT This one run before LOAD SAVE for sure because in there we set wait for 1 frame then Load
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(0f, _healthPoints - damage);

            if (_healthPoints <= 0f)
            {
                DeathBehaviour();
            }
        }

        public float GetPercentage()
        {
            return Mathf.InverseLerp(0f, _baseStats.GetHealth(), _healthPoints) * 100f;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void DeathBehaviour()
        {
            if (IsDead) return;

            IsDead = true;

            _animator.SetTrigger("Die");
            _actionScheduler.StopCurrentAction();
            // NavMeshAgent Get disabled in Mover class | Can walk through because Is Kinematic need to be turn on
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return _healthPoints;
        }

        void ISaveable.RestoreState(object state) // When level loaded it get called AFTER Awake(), BEFORE Start()
        {
            _healthPoints = (float)state;

            if (_healthPoints <= 0f)
            {
                DeathBehaviour();
            }
        }
        #endregion
    }
}