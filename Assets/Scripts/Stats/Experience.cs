using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _experiencePoints = 0;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnExperienceGained;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            OnExperienceGained();
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