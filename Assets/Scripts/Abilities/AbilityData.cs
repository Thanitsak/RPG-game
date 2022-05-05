using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        #region --Properties-- (Auto)
        public GameObject User { get; private set; }
        public IEnumerable<GameObject> Targets { get; set; }
        public Vector3 TargetedPoint { get; set; }
        #endregion



        #region --Constructors-- (PUBLIC)
        public AbilityData(GameObject user)
        {
            User = user;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartCoroutine(IEnumerator enumerator)
        {
            User.transform.root.GetComponentInChildren<MonoBehaviour>().StartCoroutine(enumerator); // Pick any MonoBehaviour script to call StartCoroutine()
        }
        #endregion
    }
}