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
        private Dictionary<string, DialogueNode> _nodeLookUpTable = new Dictionary<string, DialogueNode>();
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
                DialogueNode rootNode = new DialogueNode();
                rootNode.Text = "type dialogue script here... (root)";

                _nodes.Add(rootNode);
            }
#endif
            UpdateLookUpTable();
        }

        private void OnValidate()
        {
            UpdateLookUpTable();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.Children)
            {
                if (!_nodeLookUpTable.ContainsKey(childID)) continue;

                yield return _nodeLookUpTable[childID];
            }
        }

        public void CreateChildNode(DialogueNode parentNode)
        {
            DialogueNode childNode = new DialogueNode();
            childNode.Text = "type consequence dialogue script here...";
            parentNode.Children.Add(childNode.UniqueID);

            _nodes.Add(childNode);
            
            UpdateLookUpTable();
        }

        public void DeleteItselfNode(DialogueNode nodeToDelete)
        {
            _nodes.Remove(nodeToDelete);

            UpdateLookUpTable();

            CleanDanglingNode(nodeToDelete);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateLookUpTable()
        {
            _nodeLookUpTable.Clear();

            foreach (DialogueNode eachParentNode in _nodes)
            {
                _nodeLookUpTable.Add(eachParentNode.UniqueID, eachParentNode);
            }
        }

        private void CleanDanglingNode(DialogueNode nodeToDelete)
        {
            // Remove this node from any of the other node's children list
            foreach (DialogueNode eachNode in _nodes)
            {
                eachNode.Children.Remove(nodeToDelete.UniqueID);
            }
        }
        #endregion
    }
}