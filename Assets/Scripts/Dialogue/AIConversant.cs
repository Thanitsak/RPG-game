using UnityEngine;
using RPG.Control;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Dialogue
{
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
            _actionScheduler = GameObject.FindWithTag("Player").GetComponent<ActionScheduler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_playerConversant == null || GetComponent<Health>().IsDead) return;

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
            if (!enabled || GetComponent<Health>().IsDead) return false;

            if (Input.GetMouseButtonDown(0))
            {
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);

                _playerConversant = playerController.GetComponent<PlayerConversant>();
            }
            return true;
        }
        #endregion
    }
}