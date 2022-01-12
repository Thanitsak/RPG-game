using UnityEngine;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _healthPoints = 100f;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Animator _animator;
        #endregion



        #region --Properties-- (Auto)
        public bool IsDead { get; private set; } = false;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
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
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void DeathBehaviour()
        {
            if (IsDead) return;

            IsDead = true;

            _animator.SetTrigger("Die");
            _actionScheduler.StopCurrentAction();
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