using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _experiencePoints = 0;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
        }

        public float GetExperiencePoints()
        {
            return _experiencePoints;
        }
        #endregion



        #region --Methods-- (Interface)
        public object CaptureState()
        {
            return _experiencePoints;
        }

        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
        #endregion
    }
}