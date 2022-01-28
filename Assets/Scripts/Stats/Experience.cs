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
        public event Action OnExperienceLoaded;
        #endregion



        #region --Properties-- (With Backing Fields)
        public float ExperiencePoints { get { return _experiencePoints; } private set { _experiencePoints = value; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void GainExperience(float experience)
        {
            ExperiencePoints += experience;
            OnExperienceGained?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return ExperiencePoints;
        }

        void ISaveable.RestoreState(object state)
        {
            ExperiencePoints = (float)state;

            OnExperienceLoaded?.Invoke(); // Update CurrentLevel After Load XP
        }
        #endregion
    }
}