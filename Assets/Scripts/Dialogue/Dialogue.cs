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



        #region --Properties-- (With Backing Fields)
        public IEnumerable<DialogueNode> Nodes { get { return _nodes; } }
        #endregion



#if UNITY_EDITOR
        #region --Methods-- (Built In)
        private void Awake()
        {
            if (_nodes.Count == 0)
            {
                DialogueNode defaultNode = new DialogueNode();
                defaultNode.text = "default dialogue";

                _nodes.Add(defaultNode);
            }
        }
        #endregion
#endif



        #region --Methods-- (Custom PUBLIC)
        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }
        #endregion
    }
}