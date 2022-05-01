using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public abstract class TargetingStrategy : ScriptableObject
    {
        #region --Methods-- (Custom PUBLIC)
        public abstract void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> onFinished);
        #endregion
    }
}