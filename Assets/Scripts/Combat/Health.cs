using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _healthPoints = 100f;
        #endregion



        #region --Fields-- (In Class)
        private bool _isDead = false;

        private Animator _animator;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
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
            if (_isDead) return;
            
            _animator.SetTrigger("Die");
            _isDead = true;
        }
        #endregion
    }
}