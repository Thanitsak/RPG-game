using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _speed = 5f;
        #endregion



        #region --Methods-- (Built In)
        private void Update()
        {
            if (_target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }
        #endregion



        #region --Methods-- (Custom)
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return _target.position;

            print(_target.position + (Vector3.up * (targetCapsule.height / 2f)));
            return _target.position + (Vector3.up * (targetCapsule.height / 2f));
        }
        #endregion
    }
}