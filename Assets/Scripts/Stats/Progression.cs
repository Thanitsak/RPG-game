using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/ New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private ProgressionCharacterType[] _chractersProgression;
        #endregion



        #region --Fields-- (In Class)
        private Dictionary<CharacterType, Dictionary<StatType, float[]>> _progressionTable = null;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetStat(CharacterType characterType, StatType statType, int currentLevel)
        {
            CreateLookupTable();

            float[] levels = _progressionTable[characterType][statType];

            if (currentLevel > levels.Length) return 0f; // Guard Check for Index OutofBound

            return levels[currentLevel - 1];
        }

        public int GetLevelsLength(CharacterType characterType, StatType statType)
        {
            CreateLookupTable();

            float[] levels = _progressionTable[characterType][statType];
            return levels.Length;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CreateLookupTable()
        {
            if (_progressionTable != null) return; // Only Create Table Once

            _progressionTable = new Dictionary<CharacterType, Dictionary<StatType, float[]>>();

            foreach (ProgressionCharacterType characterProgression in _chractersProgression)
            {
                _progressionTable.Add(characterProgression.characterType, new Dictionary<StatType, float[]>());

                foreach (ProgressionStatType statProgression in characterProgression.statProgression)
                {
                    _progressionTable[characterProgression.characterType].Add(statProgression.statType, statProgression.levels);
                }
            }
            
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class ProgressionCharacterType
        {
            public CharacterType characterType;

            public ProgressionStatType[] statProgression;
        }

        [System.Serializable]
        private class ProgressionStatType
        {
            public StatType statType;

            public float[] levels;
        }
        #endregion
    }
}