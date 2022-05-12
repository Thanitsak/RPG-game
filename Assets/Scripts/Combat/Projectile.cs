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
        private Vector3 _targetPoint;
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
            if (_target != null && _isHoming && !_target.IsDead)
                transform.LookAt(GetAimLocation());

            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (health == null || health.IsDead) return; // Make sure the hit gameObject's Health is Not null AND Not Yet Dead
            if (_target != null && _target != health) return; // Make sure Specified Target (Incase not null) is Same as the hit gameObject's Health
            if (other.gameObject == _attacker) return; // Make sure Projectile is Not colliding with ourselves

            health.TakeDamage(_attacker, _damage);

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
        public void SetTarget(GameObject attacker, float damage, Health target)
        {
            SetTarget(attacker, damage, target, default);
        }

        public void SetTarget(GameObject attacker, float damage, Vector3 targetPoint)
        {
            SetTarget(attacker, damage, null, targetPoint);
        }

        public void SetTarget(GameObject attacker, float damage, Health target=null, Vector3 targetPoint=default)
        {
            _attacker = attacker;
            _damage = damage;
            _target = target;
            _targetPoint = targetPoint;

            Destroy(gameObject, _maxLifeTime);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private Vector3 GetAimLocation()
        {
            if (_target == null) return _targetPoint;

            // Make it shoot at middle of Target body
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return _target.transform.position;

            return _target.transform.position + (Vector3.up * (targetCapsule.height / 2f));
        }
        #endregion
    }
}