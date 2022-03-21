using UnityEngine;
using RPG.Dialogue;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Fighter[] _fighters;
        [SerializeField] private bool _isActivateOnStart = false;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            Activate(_isActivateOnStart);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Activate(bool status)
        {
            foreach (Fighter eachFighter in _fighters)
            {
                eachFighter.enabled = status;

                CombatTarget combatTarget = eachFighter.GetComponent<CombatTarget>();
                if (combatTarget != null) combatTarget.enabled = status;

                AIConversant aiConversant = eachFighter.GetComponent<AIConversant>();
                if (aiConversant != null) aiConversant.enabled = !status;
            }
        }
        #endregion
    }
}