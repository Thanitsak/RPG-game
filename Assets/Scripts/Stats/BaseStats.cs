using UnityEngine;
using System;
using System.Collections.Generic;

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
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLevelUp;
        #endregion



        #region --Fields-- (In Class)
        private Experience _experience;

        private int _currentLevel = 0;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _experience = GetComponent<Experience>();

            if (_experience != null)
            {
                _experience.OnExperienceGained += UpdateLevel;
                _experience.OnExperienceLoaded += RefreshCurrentLevel;
            }
            
            _currentLevel = CalculateLevel();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth() => _progression.GetStat(_characterType, StatType.Health, GetLevel()) + GetAdditiveModifier(StatType.Health);

        public float GetExperienceReward() => _progression.GetStat(_characterType, StatType.ExperienceReward, GetLevel()) + GetAdditiveModifier(StatType.ExperienceReward);

        public float GetDamage() => _progression.GetStat(_characterType, StatType.Damage, GetLevel()) + GetAdditiveModifier(StatType.Damage);

        public int GetLevel()
        {
            if (_currentLevel < 1) // Initialized CurrentLevel first before using it, incase it not yet initialized
            {
                _currentLevel = CalculateLevel();
            }

            return _currentLevel;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private float GetAdditiveModifier(StatType statType)
        {
            float total = 0f;

            foreach (IModifierProvider eachProvider in GetComponents<IModifierProvider>()) // Get All Classes that Implement IModifierProvider attached on this gameObject
            {
                foreach (float eachModifier in eachProvider.GetAdditiveModifier(statType))
                {
                    total += eachModifier;
                }
            }

            return total;
        }

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
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel; // Have to Run First! Cuz OnLevelUp will use _currentLevel new value

                OnLevelUp?.Invoke();
                LevelUpEffect();
            }
        }

        private void RefreshCurrentLevel()
        {
            _currentLevel = CalculateLevel();
        }
        #endregion
    }
}