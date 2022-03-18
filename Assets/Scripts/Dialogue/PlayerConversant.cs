using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Dialogue _currentDialogue;
        #endregion



        #region --Fields-- (In Class)
        private DialogueNode _currentNode;
        private DialogueNode _aiNode;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _currentNode = _currentDialogue.GetRootNode();

            if (_currentDialogue.GetRootNode().Speaker == DialogueSpeaker.AI)
                _aiNode = _currentDialogue.GetRootNode();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
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
            if (_aiNode == null)
            {
                return "";
            }

            return _aiNode.QuestionText;
        }

        public IEnumerable<string> GetChoices()
        {
            DialogueNode[] childArray = _currentDialogue.GetAllChildren(_aiNode).ToArray();

            foreach (DialogueNode eachNode in childArray)
            {
                yield return eachNode.Text;
            }
        }

        public void Next()
        {
            if (!HasNext()) return;

            DialogueNode[] childArray = _currentDialogue.GetAllChildren(_currentNode).ToArray();

            DialogueNode randChild = childArray[Random.Range(0, childArray.Length)];

            _currentNode = randChild;
            if (_currentNode.Speaker == DialogueSpeaker.AI)
                _aiNode = _currentNode;
        }

        public bool HasNext()
        {
            return _currentDialogue.GetAllChildren(_currentNode).ToArray().Length > 0;
        }

        public bool IsPlayerSpeaking()
        {
            return _currentNode.Speaker == DialogueSpeaker.Player;
        }
        #endregion
    }
}