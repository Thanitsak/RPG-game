using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using RPG.Core;

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



        #region --Methods-- (Custom PUBLIC) ~dealing with nodes, dialogue stuff~
        public void StartDialogue(AIConversant newAIConversant, Dialogue newDialogue)
        {
            _aiConversant = newAIConversant;
            _currentDialogue = newDialogue;

            // Randomly get a Node from All the Root
            DialogueNode[] allRoot = FilterOnCondition(_currentDialogue.GetRootNodes()).ToArray();
            DialogueNode randRoot = allRoot[UnityEngine.Random.Range(0, allRoot.Length)];

            _previousNode = randRoot; // Incase root is Player Node so GetChoices() and GetQuestText() can work
            _currentNode = randRoot;

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
            DialogueNode[] playerNode = FilterOnCondition(_currentDialogue.GetPlayerChildren(_previousNode)).ToArray();
            
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
            DialogueNode[] allNode = FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).ToArray();
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
            return FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).Count() > 0;
        }

        public bool IsActive()
        {
            return _currentDialogue != null;
        }

        public bool IsPlayerSpeaking()
        {
            return _currentNode.Speaker == DialogueSpeaker.Player;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~AIConversant Getter~
        public string GetAISpeakerName() => _aiConversant.SpeakerName;
        public Sprite GetAIProfileImage() => _aiConversant.ProfileImage;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Node Filtering~
        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes)
        {
            IEnumerable<IPredicateEvaluator> allEvaluators = GetEvaluators();

            foreach (DialogueNode eachNode in inputNodes)
            {
                if (eachNode.CheckCondition(allEvaluators))
                {
                    yield return eachNode;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            List<IPredicateEvaluator> _classesThatImplemented = new List<IPredicateEvaluator>();

            foreach (IPredicateEvaluator each in transform.root.GetComponentsInChildren<IPredicateEvaluator>()) // Get from Player GameObject (Ex-Invenotry.cs, QuestList.cs)
                _classesThatImplemented.Add(each);

            foreach (IPredicateEvaluator each in _aiConversant.transform.parent.GetComponentsInChildren<IPredicateEvaluator>()) // Get from AI GameObject that Player is talking to (Ex-RewardGiver.cs)
                _classesThatImplemented.Add(each);

            return _classesThatImplemented;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Trigger Action Stuff~
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

            foreach (DialogueTrigger eachDialogueTrigger in _aiConversant.transform.parent.GetComponentsInChildren<DialogueTrigger>())
            {
                eachDialogueTrigger.Trigger(actionString);
            }
        }
        #endregion
    }
}