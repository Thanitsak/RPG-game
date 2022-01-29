using UnityEngine;
using System;
using BestVoxels.Utils;

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
        public event Action OnLevelUp;
        #endregion



        #region --Fields-- (In Class)
        private Experience _experience;

        private AutoInit<int> _currentLevel;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _experience = GetComponent<Experience>();

            _currentLevel = new AutoInit<int>(GetInitialCurrentLevel);
        }

        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained += UpdateLevel;
                _experience.OnExperienceLoaded += RefreshCurrentLevel;
            }
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnDisable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained -= UpdateLevel;
                _experience.OnExperienceLoaded -= RefreshCurrentLevel;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth() => GetStat(StatType.Health);

        public float GetExperienceReward() => GetStat(StatType.ExperienceReward);

        public float GetDamage() => GetStat(StatType.Damage);

        public int GetLevel() => _currentLevel.value;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Stats Formula~
        private float GetStat(StatType statType)
        {
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
            foreach (IModifierProvider eachProvider in GetComponents<IModifierProvider>()) // Get All Classes that Implement IModifierProvider attached on this gameObject
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
            foreach (IModifierProvider eachProvider in GetComponents<IModifierProvider>()) // Get All Classes that Implement IModifierProvider attached on this gameObject
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
        private int GetInitialCurrentLevel() => CalculateLevel();

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel; // Have to Run First! Cuz OnLevelUp will use _currentLevel new value

                OnLevelUp?.Invoke();
                LevelUpEffect();
            }
        }

        private void RefreshCurrentLevel()
        {
            _currentLevel.value = CalculateLevel();
        }
        #endregion
    }
}