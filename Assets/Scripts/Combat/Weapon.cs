using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private GameObject _equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private float _damage = 10f;
        [Tooltip("How Close the character need to walk near opponent in order to deal damange.")]
        [SerializeField] private float _range = 2f;
        [SerializeField] private bool _isRightHanded = true;
        [SerializeField] private Projectile _projectile = null;
        #endregion



        #region --Fields-- (In Class)
        private const string _weaponName = "Weapon";
        #endregion



        #region --Properties-- (With Backing Fields)
        public float Damage { get { return _damage; } }
        public float Range { get { return _range; } }
        public bool HasProjectile { get { return _projectile != null; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }

            if (_equippedPrefab != null)
            {
                GameObject weaponCreated = Instantiate(_equippedPrefab, GetTransform(rightHand, leftHand));
                weaponCreated.name = _weaponName;
            }
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileCloned = Instantiate(_projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileCloned.SetTarget(target, _damage);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(_weaponName);

            if (oldWeapon == null) oldWeapon = leftHand.Find(_weaponName);
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING"; // for not having confusing from new Instantiated Weapon
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (_isRightHanded)
                handTransform = rightHand;
            else
                handTransform = leftHand;

            return handTransform;
        }
        #endregion
    }
}