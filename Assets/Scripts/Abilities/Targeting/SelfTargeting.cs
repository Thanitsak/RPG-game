using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Untitled Self Targeting", menuName = "RPG/Game Item/Targeting/New Self", order = 127)]
    public class SelfTargeting : TargetingStrategy
    {
        #region --Methods-- (Override)
        public override void StartTargeting(AbilityData data, Action onFinished)
        {
            data.TargetedPoint = data.User.transform.position;
            data.Targets = new GameObject[] { data.User };

            onFinished?.Invoke();
        }
        #endregion
    }
}