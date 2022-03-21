using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public event Action OnDialogueUpdated;
        #endregion



        #region --Fields-- (In Class)
        private AIConversant _aiConversant;
        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;
        private DialogueNode _previousNode;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartDialogue(AIConversant newAIConversant, Dialogue newDialogue)
        {
            _aiConversant = newAIConversant;
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();

            if (!IsPlayerSpeaking())
                TriggerEnterAction();

            OnDialogueUpdated?.Invoke();
        }

        public void QuitDialogue()
        {
            if (!IsPlayerSpeaking())
                TriggerExitAction();

            _aiConversant = null;
            _currentDialogue = null;
            _currentNode = null;
            _previousNode = null;

            OnDialogueUpdated?.Invoke();
        }

        public string GetText()
        {
            if (_currentNode == null)
            {
                return "";
            }

            return _currentNode.Text;
        }

        public string GetQuestionText()
        {
            if (_previousNode == null)
            {
                return "";
            }

            return _previousNode.QuestionText;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            DialogueNode[] playerNode = _currentDialogue.GetPlayerChildren(_previousNode).ToArray();

            foreach (DialogueNode eachNode in playerNode)
            {
                yield return eachNode;
            }
        }

        public void GetChoiceNode(DialogueNode clickedNode)
        {
            _previousNode = _currentNode;
            _currentNode = clickedNode;

            TriggerEnterAction();
            OnDialogueUpdated?.Invoke();

            GetNextNode();
        }

        public void GetNextNode()
        {
            if (!HasNext())
            {
                QuitDialogue();
                return;
            }

            // Randomly get Node from All Children
            DialogueNode[] allNode = _currentDialogue.GetAllChildren(_currentNode).ToArray();
            DialogueNode randChild = allNode[UnityEngine.Random.Range(0, allNode.Length)];

            TriggerExitAction();

            _previousNode = _currentNode;
            _currentNode = randChild;

            if (!IsPlayerSpeaking())
                TriggerEnterAction();

            OnDialogueUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return _currentDialogue.GetAllChildren(_currentNode).ToArray().Length > 0;
        }

        public bool IsActive()
        {
            return _currentDialogue != null;
        }

        public bool IsPlayerSpeaking()
        {
            return _currentNode.Speaker == DialogueSpeaker.Player;
        }

        public string GetAISpeakerName() => _aiConversant.SpeakerName;
        public Sprite GetAIProfileImage() => _aiConversant.ProfileImage;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        // When Calling Above, some need to check IF currentNode is PlayerNode CUZ we don't want to trigger action right away when PlayerNode choice is not yet picked OR when exit
        // the current node will be one of player choice when get randomed, so only trigger enter normal node right away AND trigger exit when leave the current node.
        private void TriggerEnterAction()
        {
            if (_currentNode == null) return;

            TriggerAction(_currentNode.OnTriggerEnter);
        }

        private void TriggerExitAction()
        {
            if (_currentNode == null) return;

            TriggerAction(_currentNode.OnTriggerExit);
        }

        private void TriggerAction(string actionString)
        {
            if (actionString == "") return;

            foreach (DialogueTrigger eachDialogueTrigger in _aiConversant.GetComponents<DialogueTrigger>())
            {
                eachDialogueTrigger.Trigger(actionString);
            }
        }
        #endregion
    }
}