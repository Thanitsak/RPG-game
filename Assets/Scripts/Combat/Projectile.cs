using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _speed = 20f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private float _maxLifeTime = 10f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private float _lifeAfterHit = 0.5f;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("UnityEvent")]
        [SerializeField] private UnityEvent _onProjectileLaunch;
        [SerializeField] private UnityEvent _onProjectileHit;
        #endregion



        #region --Fields-- (In Class)
        private Health _target = null;
        private float _damage = 0f;
        private GameObject _attacker = null;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            transform.LookAt(GetAimLocation());
            _onProjectileLaunch?.Invoke();
        }

        private void Update()
        {
            if (_target == null) return;

            if (_isHoming && !_target.IsDead)
                transform.LookAt(GetAimLocation());

            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target || _target.IsDead) return;

            _target.TakeDamage(_attacker, _damage);
            _onProjectileHit?.Invoke();

            if (_hitEffect != null)
                Instantiate(_hitEffect, other.ClosestPointOnBounds(transform.position), transform.rotation);

            foreach (GameObject each in _destroyOnHit)
            {
                Destroy(each);
            }

            _speed = 0f;

            Destroy(gameObject, _lifeAfterHit);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetTarget(GameObject attacker, Health target, float damage)
        {
            _target = target;
            _damage = damage;
            _attacker = attacker;

            Destroy(gameObject, _maxLifeTime);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private Vector3 GetAimLocation()
        {
            // Make it shoot at middle of Target body
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return _target.transform.position;

            return _target.transform.position + (Vector3.up * (targetCapsule.height / 2f));
        }
        #endregion
    }
}