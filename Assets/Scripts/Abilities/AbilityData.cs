using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using RPG.Core;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        // Reason to Implement IAction here and not at Ability.cs is because a single Ability might be shared between multiple characters
        // ,and Since purpose of IAction need to be on Per Character so when Cancel() get called it only cancel Action of that specific Character.
        // So AbilityData.cs is perfect place since it will be created newly for a specific user.
        // Ex - Fighter or Mover components individually attached per character.

        #region --Properties-- (Auto)
        public GameObject User { get; private set; }
        public IEnumerable<GameObject> Targets { get; set; }
        public Vector3 TargetedPoint { get; set; }
        public bool IsAbilityCancelled { get; private set; } = false; // use for checking in IEnumerator of Targeting & Effects
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



        #region --Methods-- (Interface)
        void IAction.Cancel()
        {
            IsAbilityCancelled = true;
        }
        #endregion
    }
}