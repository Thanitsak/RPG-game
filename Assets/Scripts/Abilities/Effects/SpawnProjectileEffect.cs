using System;
using UnityEngine;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Untitled Spawn Projectile Effect", menuName = "RPG/Game Item/Effects/New Spawn Projectile Effect", order = 133)]
    public class SpawnProjectileEffect : EffectStrategy
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Projectile _projectile;
        [Tooltip("Only provide positive value, to deduct health with amount provide.")]
        [SerializeField] private float _damage = 0f;
        [Tooltip("Using Hands Transform that provided in Fighter script. (to add more spawning point, Check WeaponConfig for how to do that.)")]
        [SerializeField] private bool _isRightHanded = true;
        [Tooltip("Should this Projectile only Launch when there are Targets OR just launch to the TargetPoint? true mean just launch to target point otherwise false")]
        [SerializeField] private bool _useTargetPoint = true;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private Transform GetTransform(Transform rightHand, Transform leftHand) => _isRightHanded ? rightHand : leftHand;

        private void SpawnProjectileForTargets(AbilityData data, Transform handTransform)
        {
            foreach (GameObject target in data.Targets)
            {
                Health health = target.GetComponentInChildren<Health>();
                if (health == null || health.IsDead) continue;

                Projectile projectileCloned = Instantiate(_projectile, handTransform.position, handTransform.rotation);
                projectileCloned.SetTarget(data.User, _damage, health);
            }
        }

        private void SpawnProjectileForTargetPoint(AbilityData data, Transform handTransform)
        {
            Projectile projectileCloned = Instantiate(_projectile, handTransform.position, handTransform.rotation);
            projectileCloned.SetTarget(data.User, _damage, data.TargetedPoint);
        }
        #endregion



        #region --Methods-- (Override)
        public override void StartEffect(AbilityData data, Action<string> onFinished)
        {
            Fighter fighter = data.User.transform.root.GetComponentInChildren<Fighter>();
            if (fighter == null) return;
            Transform handTransform = GetTransform(fighter.RightHandTransform, fighter.LeftHandTransform);

            if (_useTargetPoint)
            {
                SpawnProjectileForTargetPoint(data, handTransform);
            }
            else
            {
                SpawnProjectileForTargets(data, handTransform);
            }

            onFinished?.Invoke(name);
        }
        #endregion
    }
}