using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    /// <summary>
    /// *Attachment Level Position Under A GameObject*
    /// 
    /// PlayerConversant.cs
    /// - can be placed anywhere as a child of 'Player' GameObject. (due to using .root when getting its component, CAN use .root since under a single root there is only a 'Player' GameObject)
    ///
    /// AIConversant.cs
    /// - MUST be placed 1 level underneath as a child of 'AI' GameObject. (due to using .parent when getting its component, CAN'T use .root since under a single root there are many 'AI' GameObjects)
    /// - Have to Be Strict becuase itself class use transform.parent.GetComponent<Health>() & PlayerConversant.cs use _aiConversant.transform.parent.GetComponentsInChildren<IPredicateEvaluator>()
    /// 
    /// DialogueTrigger.cs
    /// - can be in same place as AIConversant.cs but Not as Strict as AIConversant.cs
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _actionString;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("UnityEvent")]
        [SerializeField] private UnityEvent _onTriggerHappen;
        #endregion



        #region --Methods (Custom PUBLIC)
        public void Trigger(string callerActionString)
        {
            if (_actionString == callerActionString)
            {
                _onTriggerHappen?.Invoke();
            }
        }
        #endregion
    }
}