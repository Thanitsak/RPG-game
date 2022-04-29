using UnityEngine;

namespace RPG.Abilities
{
    public abstract class TargetingStrategy : ScriptableObject
    {
        #region --Methods-- (Custom PUBLIC)
        public abstract void StartTargeting();
        #endregion
    }
}