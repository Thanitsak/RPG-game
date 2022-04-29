using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Untitled Targeting", menuName = "RPG/Game Item/Targeting/New Targeting (TargetingStrategy)", order = 125)]
    public class DemoTargeting : TargetingStrategy
    {
        #region --Methods-- (Override)
        public override void StartTargeting()
        {
            Debug.Log("Demo Targeting Started");
        }
        #endregion
    }
}