using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _speed = 20f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private GameObject _hitEffect = null;
        #endregion



        #region --Fields-- (In Class)
        private Health _target = null;
        private float _damage = 0f;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            transform.LookAt(GetAimLocation());
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

            if (_hitEffect != null)
                Instantiate(_hitEffect, other.ClosestPointOnBounds(transform.position), transform.rotation);

            _target.TakeDamage(_damage);

            Destroy(gameObject);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
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