using UnityEngine;
using RPG.Inventories;
using RPG.Attributes;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Untitled Ability", menuName = "RPG/Game Item/New Ability (Action)", order = 124)]
    public class Ability : ActionItem
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TargetingStrategy _targetingStrategy;
        [SerializeField] private FilterStrategy[] _filterStrategies;
        [SerializeField] private EffectStrategy[] _effectStrategies;
        [Min(0f)]
        [SerializeField] private float _cooldownTime = 2f;
        [SerializeField] private float _manaCost = 20f;
        #endregion



        #region --Fields-- (In Class)
        private CooldownStore _cooldownStore;
        private Mana _mana;
        #endregion



        #region --Methods-- (Override)
        public override void Use(GameObject user)
        {
            if (_targetingStrategy == null || _filterStrategies.Length == 0 || _effectStrategies.Length == 0) return;
            
            _cooldownStore = user.transform.root.GetComponentInChildren<CooldownStore>();
            if (_cooldownStore == null || _cooldownStore.GetTimeRemaining(this) > 0f) return;

            _mana = user.transform.root.GetComponentInChildren<Mana>();
            if (_mana == null || _manaCost > _mana.ManaPoints.value) return;

            AbilityData data = new AbilityData(user);
            _targetingStrategy.StartTargeting(data, () => OnTargetAquired(data));
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void OnTargetAquired(AbilityData data)
        {
            if (_mana.UseMana(_manaCost) == false) return;

            _cooldownStore.StartTimer(this, _cooldownTime);

            foreach (FilterStrategy eachFilter in _filterStrategies)
            {
                data.Targets = eachFilter.Filter(data.Targets);
            }

            foreach (EffectStrategy eachEffect in _effectStrategies)
            {
                eachEffect.StartEffect(data, OnEffectFinished);
            }
        }

        private void OnEffectFinished()
        {
            Debug.Log("On Effect Finished");
        }
        #endregion
    }
}