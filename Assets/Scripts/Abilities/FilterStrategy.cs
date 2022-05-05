using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public abstract class FilterStrategy : ScriptableObject
    {
        #region --Methods-- (Custom PUBLIC)
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> targetsToFilter);
        #endregion
    }
}