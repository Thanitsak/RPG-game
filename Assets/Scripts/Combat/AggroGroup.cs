using UnityEngine;
using RPG.Dialogue;
using RPG.Saving;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Fighter[] _fighters;
        [SerializeField] private bool _isActivateOnStart = false;
        #endregion



        #region --Fields-- (In Class)
        private bool _aggroStatus = false;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            Activate(_isActivateOnStart);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void Activate(bool status)
        {
            _aggroStatus = status;

            foreach (Fighter eachFighter in _fighters)
            {
                eachFighter.enabled = status;

                CombatTarget combatTarget = eachFighter.GetComponentInChildren<CombatTarget>();
                if (combatTarget != null) combatTarget.enabled = status;

                AIConversant aiConversant = eachFighter.GetComponentInChildren<AIConversant>();
                if (aiConversant != null) aiConversant.enabled = !status;
            }
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return _aggroStatus;
        }

        void ISaveable.RestoreState(object state)
        {
            _aggroStatus = (bool)state;

            Activate(_aggroStatus);
        }
        #endregion
    }
}