using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/ New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private ProgressionCharacterType[] _chractersProgression;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public float GetHealth(CharacterType characterType, int currentLevel)
        {
            foreach (ProgressionCharacterType characterProgression in _chractersProgression)
            {
                if (characterProgression.characterType == characterType)
                {
                    return characterProgression.health[currentLevel - 1];
                }
            }

            return 0f;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class ProgressionCharacterType
        {
            public CharacterType characterType;

            public int[] health;
            public int[] damage;
        }
        #endregion
    }
}