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



        #region --Properties-- (With Backing Fields)
        public float Damage { get { return _damage; } }
        public float Range { get { return _range; } }
        public bool HasProjectile { get { return _projectile != null; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }

            if (_equippedPrefab != null)
            {
                Instantiate(_equippedPrefab, GetTransform(rightHand, leftHand));
            }
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileCloned = Instantiate(_projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileCloned.SetTarget(target);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
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