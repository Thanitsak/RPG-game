using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using RPG.Inventories;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Untitled Weapon", menuName = "RPG/Game Item/New Weapon (Equipable)", order = 113)]
    public class WeaponConfig : EquipableItem, IModifierProvider
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
        private GameObject _launcherCreated = null;
        #endregion



        #region --Properties-- (With Backing Fields)
        public float Damage { get { return _damage; } }
        public float DamageBonusPercentage { get { return _damageBonusPercentage; } }
        public float Range { get { return _range; } }
        public bool HasProjectile { get { return _projectile != null; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
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

            Weapon weaponCreated = null;
            if (_equippedPrefab != null)
            {
                weaponCreated = Instantiate(_equippedPrefab, GetTransform(rightHand, leftHand));
                weaponCreated.gameObject.name = _weaponName;

                if (_projectileLauncherPrefab != null)
                    _launcherCreated = Instantiate(_projectileLauncherPrefab, weaponCreated.transform);
            }

            return weaponCreated;
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



        #region --Methods-- (Interface)
        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatType statType)
        {
            if (statType == StatType.Damage)
            {
                yield return Damage;
                // This way it's concisely to say that we want to return something Otherwise return nothing or as empty list since we are using IEnumerable it's handy
                // We can also return more than one thing by doing 'yield return _anotherCurrenetWeapon.Damage;' as it's allow in IEnumerable
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatType statType)
        {
            if (statType == StatType.Damage)
            {
                yield return DamageBonusPercentage;
            }
        }
        #endregion
    }
}