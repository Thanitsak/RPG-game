using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Weapon _equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private float _damage = 10f;
        [Range(0f, 100f)]
        [SerializeField] private float _damageBonusPercentage = 0f;
        [Tooltip("How Close the character need to walk near opponent in order to deal damange.")]
        [SerializeField] private float _range = 2f;
        [SerializeField] private bool _isRightHanded = true;
        [SerializeField] private Projectile _projectile = null;
        [SerializeField] private GameObject _projectileLauncherPrefab = null;
        #endregion



        #region --Fields-- (In Class)
        private const string _weaponName = "Weapon";
        private GameObject _weaponCreated = null;
        private GameObject _launcherCreated = null;
        #endregion



        #region --Properties-- (With Backing Fields)
        public float Damage { get { return _damage; } }
        public float DamageBonusPercentage { get { return _damageBonusPercentage; } }
        public float Range { get { return _range; } }
        public bool HasProjectile { get { return _projectile != null; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            if (_equippedPrefab != null)
            {
                _weaponCreated = Instantiate(_equippedPrefab.gameObject, GetTransform(rightHand, leftHand));
                _weaponCreated.name = _weaponName;

                if (_projectileLauncherPrefab != null)
                    _launcherCreated = Instantiate(_projectileLauncherPrefab, _weaponCreated.transform);
            }
        }

        public void LaunchProjectile(GameObject attacker, Health target, float calculatedDamage)
        {
            Projectile projectileCloned = Instantiate(_projectile, _launcherCreated.transform.position, _launcherCreated.transform.rotation); // Create from LauncherPosition instead of using Transform as quippedWeapon because Projectile can't have parent otherwise it's going to move according to player hand shake
            projectileCloned.SetTarget(attacker, target, calculatedDamage);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(_weaponName);

            if (oldWeapon == null) oldWeapon = leftHand.Find(_weaponName);
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING"; // for not having Destroy Confusion with new Instantiated Weapon
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