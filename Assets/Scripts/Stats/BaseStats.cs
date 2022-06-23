using UnityEngine;
using System;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Range(1, 99)]
        [SerializeField] private int _startingLevel = 1;
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private Progression _progression = null;
        [SerializeField] private GameObject _levelUpParticleEffect = null;
        [SerializeField] private bool _shouldUseModifiers = false;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLevelUpSetup; // Purpose : use for subscriber that will make effect on value for their fields, ex-RegenerateHealth() or UpdateMaxManaPoints(). So Should Not be used for Refreshing UI since order of subscribers, subscriber that effect the value might run after Refreshing UI subscriber.
        public event Action OnLevelUpDone; // Purpose : use for subscriber that will only refreshing UI, make no effect on value. So can guarantee no subscriber will make an effect on the value, so can use for Refreshing UI.
        #endregion



        #region --Fields-- (In Class)
        private Experience _experience;

        private AutoInit<int> _currentLevel;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _experience = GetComponent<Experience>();

            _currentLevel = new AutoInit<int>(CalculateLevel);
        }

        private void OnEnable()
        {
            if (_experience == null) return;
            _experience.OnExperienceGained += UpdateLevel;
            _experience.OnRefreshCurrentLevelOnly += RefreshCurrentLevel;
            
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnDisable()
        {
            if (_experience == null) return;
            _experience.OnExperienceGained -= UpdateLevel;
            _experience.OnRefreshCurrentLevelOnly -= RefreshCurrentLevel;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth() => GetStat(StatType.Health);
        public float GetExperienceReward() => GetStat(StatType.ExperienceReward);
        public float GetDamage() => GetStat(StatType.Damage);
        public float GetDefence() => GetStat(StatType.Defence);

        public float GetMana() => GetStat(StatType.Mana);
        public float GetManaRegenRate() => GetStat(StatType.ManaRegenRate);

        public float GetTotalTraitPoint() => GetStat(StatType.TotalTraitPoints);

        // No BaseStats value Included in Progression scriptable object for these bcuz we only use its addtive or percentage(can't use cuz * zero will be zero) modifier NOT its base value.
        public float GetOnTopBuyingDiscountPercentage() => GetStat(StatType.OnTopBuyingDiscountPercentage);
        public float GetOnTopSellingDiscountPercentage() => GetStat(StatType.OnTopSellingDiscountPercentage);

        public int GetLevel() => _currentLevel.value;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Stats Formula~
        private float GetStat(StatType statType)
        {
            // ADDING PERCENTAGE 'ON TOP' of Both (BASESTAT + ADDITIVE MODIFIER)
            return ((GetBaseStat(statType) + GetAdditiveModifier(statType)) * (100f + GetPercentageModifier(statType))) / 100f; // +100f cuz we want on top of the Damage we currently have
        }

        private float GetBaseStat(StatType statType)
        {
            return _progression.GetStat(_characterType, statType, GetLevel());
        }

        private float GetAdditiveModifier(StatType statType)
        {
            if (!_shouldUseModifiers) return 0f;

            float totalAdditive = 0f;
            foreach (IModifierProvider eachProvider in GetComponentsInChildren<IModifierProvider>()) // Get All Classes that Implement IModifierProvider attached on this gameObject and among children
            {
                foreach (float eachModifier in eachProvider.GetAdditiveModifiers(statType))
                {
                    totalAdditive += eachModifier;
                }
            }

            return totalAdditive;
        }

        private float GetPercentageModifier(StatType statType)
        {
            if (!_shouldUseModifiers) return 0f;

            float totalPercentage = 0f;
            foreach (IModifierProvider eachProvider in GetComponentsInChildren<IModifierProvider>()) // Get All Classes that Implement IModifierProvider attached on this gameObject and among children
            {
                foreach (float eachModifier in eachProvider.GetPercentageModifiers(statType))
                {
                    totalPercentage += eachModifier;
                }
            }

            return totalPercentage;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Level~
        private int CalculateLevel()
        {
            if (_experience == null) return _startingLevel; // Guard check for Chracter without Experience component

            int newLevel = _startingLevel;
            float currentXP = _experience.ExperiencePoints;
            float maxLevel = _progression.GetLevelsLength(_characterType, StatType.ExperienceToLevelUp);

            for (int level = 1; level <= maxLevel; level++)
            {
                if (currentXP >= _progression.GetStat(_characterType, StatType.ExperienceToLevelUp, level))
                    newLevel = (level + 1);
            }

            return newLevel;
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpParticleEffect, transform);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshCurrentLevel()
        {
            _currentLevel.value = CalculateLevel();
            
            OnLevelUpDone?.Invoke();
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel; // Have to Run First! Cuz OnLevelUpSetup's subscribers will use _currentLevel new value

                LevelUpEffect();
                OnLevelUpSetup?.Invoke();
                OnLevelUpDone?.Invoke();
            }
        }
        #endregion
    }
}