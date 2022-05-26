using UnityEngine;
using RPG.Control;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;

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
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Sprite _profileImage;
        [SerializeField] private string _speakerName;
        [SerializeField] private Dialogue _dialogue;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;
        private PlayerConversant _playerConversant;
        #endregion



        #region --Properties-- (With Backing Fields)
        public Sprite ProfileImage { get { return _profileImage; } }
        public string SpeakerName { get { return _speakerName; } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GameObject.FindWithTag("Player").GetComponentInChildren<ActionScheduler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_playerConversant == null || transform.parent.GetComponentInChildren<Health>().IsDead) return;

                _playerConversant.StartDialogue(this, _dialogue);
                _actionScheduler.StopCurrentAction();

                _playerConversant = null;
            }
        }
        #endregion



        #region --Methods-- (Interface)
        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Dialogue;
        }

        bool IRaycastable.HandleRaycast(PlayerController playerController)
        {
            if (!enabled || transform.parent.GetComponentInChildren<Health>().IsDead) return false;
            
            if (Input.GetMouseButtonDown(0))
            {
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);

                _playerConversant = playerController.GetComponentInChildren<PlayerConversant>();
            }
            return true;
        }
        #endregion
    }
}