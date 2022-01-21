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



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth() => _progression.GetStat(StatType.Health, _characterType, _startingLevel);

        public float GetExperienceReward() => _progression.GetStat(StatType.ExperienceReward, _characterType, _startingLevel);
        #endregion
    }
}