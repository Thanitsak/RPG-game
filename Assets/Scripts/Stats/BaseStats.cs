using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Range(1, 99)]
        [SerializeField] private int _startingLevel = 1;
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private Progression _progression = null;
        #endregion


        #region --Fields-- (In Class)
        private Experience _experience;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _experience = GetComponent<Experience>();
        }

        private void Update()
        {
            if (gameObject.tag == "Player")
            {
                print(GetLevel());
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth() => _progression.GetStat(_characterType, StatType.Health, GetLevel());

        public float GetExperienceReward() => _progression.GetStat(_characterType, StatType.ExperienceReward, GetLevel());

        public int GetLevel()
        {
            if (_experience == null) return _startingLevel; // Guard check for Chracter without Experience component

            int newLevel = _startingLevel;
            float currentXP = _experience.GetExperiencePoints();
            float maxLevel = _progression.GetLevelsLength(_characterType, StatType.ExperienceToLevelUp);

            for (int level = 1; level <= maxLevel; level++)
            {
                if (currentXP >= _progression.GetStat(_characterType, StatType.ExperienceToLevelUp, level))
                    newLevel = (level + 1);
            }

            return newLevel;
        }
        #endregion
    }
}