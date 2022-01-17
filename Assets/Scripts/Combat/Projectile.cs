using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _speed = 20f;
        #endregion



        #region --Fields-- (In Class)
        private Health _target = null;
        #endregion



        #region --Methods-- (Built In)
        private void Update()
        {
            if (_target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetTarget(Health target) => _target = target;
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