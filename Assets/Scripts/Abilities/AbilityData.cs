using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        #region --Properties-- (Auto)
        public GameObject User { get; private set; }
        public IEnumerable<GameObject> Targets { get; set; }
        #endregion



        #region --Constructors-- (PUBLIC)
        public AbilityData(GameObject user)
        {
            User = user;
        }
        #endregion
    }
}