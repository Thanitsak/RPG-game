using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Unnamed Dialogue", menuName = "RPG/Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();
        #endregion



        #region --Fields-- (In Class)
        private Dictionary<string, DialogueNode> _nodeTable = new Dictionary<string, DialogueNode>();
        #endregion



        #region --Properties-- (With Backing Fields)
        public IEnumerable<DialogueNode> Nodes { get { return _nodes; } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
#if UNITY_EDITOR
            if (_nodes.Count == 0)
            {
                DialogueNode defaultNode = new DialogueNode();
                defaultNode.text = "default dialogue";

                _nodes.Add(defaultNode);
            }
#endif
            OnValidate();
        }

        private void OnValidate()
        {
            _nodeTable.Clear();

            foreach (DialogueNode eachParentNode in _nodes)
            {
                _nodeTable.Add(eachParentNode.uniqueID, eachParentNode);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.children)
            {
                if (!_nodeTable.ContainsKey(childID)) continue;

                yield return _nodeTable[childID];
            }
        }
        #endregion
    }
}