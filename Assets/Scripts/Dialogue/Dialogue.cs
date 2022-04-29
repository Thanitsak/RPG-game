using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Untitled Dialogue", menuName = "RPG/Dialogue/New Dialogue", order = 2)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        #region --Fields-- (Inspector)
        [Header("Show/Hide Toggle")]
        [SerializeField] private bool _isShowedNodesAction = false;
        [SerializeField] private bool _isShowedQuestionText = true;

        [Space]
        [Space]

        [Header("All Nodes Width & Height, and create Node OffSet")]
        [SerializeField] private Vector2 _normalNodesSize = new Vector2(200, 120);
        [Space]
        [SerializeField] private float _showedActionExtraHeight = 130f;
        [Space]
        [SerializeField] private float _showedQuestionExtraHeight = 50f;
        [Space]
        [SerializeField] private float _showedConditionExtraHeight = 75f;
        [Space]
        [SerializeField] private Vector2 _newNodeOffset = new Vector2(250, 0); // No Need to create Public Properties cuz not use in DialogueEditor

        [Space]
        [Space]

        [Header("All Nodes Data in Editor")]
        [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();
        #endregion



        #region --Fields-- (In Class)
        private Dictionary<string, DialogueNode> _nodeLookUpTable = new Dictionary<string, DialogueNode>();

        private List<DialogueNode> _rootNodes = null;
        #endregion



        #region --Properties-- (With Backing Fields)
        public bool IsShowedNodesAction { get { return _isShowedNodesAction; } }
        public bool IsShowedQuestionText { get { return _isShowedQuestionText; } }

        public Vector2 NormalNodesSize { get { return _normalNodesSize; } }
        public float ShowedActionExtraHeight { get { return _showedActionExtraHeight; } }
        public float ShowedQuestionExtraHeight { get { return _showedQuestionExtraHeight; } }
        public float ShowedConditionExtraHeight { get { return _showedConditionExtraHeight; } }

        public IEnumerable<DialogueNode> Nodes { get { return _nodes; } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            UpdateLookUpTable();
            UpdateRootNodes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateLookUpTable();
            UpdateRootNodes();
        }
#endif
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public IEnumerable<DialogueNode> GetRootNodes()
        {
            if (_rootNodes == null)
                UpdateRootNodes();

            return _rootNodes;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.Children)
            {
                if (_nodeLookUpTable.ContainsKey(childID))
                {
                    yield return _nodeLookUpTable[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode eachNode in GetAllChildren(parentNode))
            {
                if (eachNode.Speaker == DialogueSpeaker.Player)
                {
                    yield return eachNode;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode eachNode in GetAllChildren(parentNode))
            {
                if (eachNode.Speaker == DialogueSpeaker.AI)
                {
                    yield return eachNode;
                }
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Mainly For Dialogue Editor~
        public void CreateChildNodeUnder(DialogueNode parentNode)
        {
            DialogueNode childNode = CreateInstance<DialogueNode>();
            childNode.name = System.Guid.NewGuid().ToString();
            childNode.Text = "Type Consequence Dialogue script here...";
            switch (parentNode.Speaker)
            {
                case DialogueSpeaker.AI:
                    childNode.Speaker = DialogueSpeaker.Player;
                    break;

                case DialogueSpeaker.Player:
                    childNode.Speaker = DialogueSpeaker.AI;
                    break;
            }
            childNode.SetRectPosition(parentNode.Rect.position + _newNodeOffset);

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(childNode, "Create Dialogue Node");
#endif

            parentNode.AddChild(childNode.name);

#if UNITY_EDITOR
            Undo.RecordObject(this, "Added Dialogue Node"); // IMPORTANT This Undo Can't Be put When creating the first root node since create root node is being call form OnBeforeSerialize() and this Undo will also call OnBeforeSerialize() so it will get infinite loop! (if want use this check //if (AssetDatabase.GetAssetPath(this) != ""))
#endif

            _nodes.Add(childNode);
            UpdateLookUpTable();
        }

        public void DeleteThisNode(DialogueNode nodeToDelete)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Deleted Dialogue Node");
#endif

            _nodes.Remove(nodeToDelete);
            UpdateLookUpTable();
            CleanDanglingNode(nodeToDelete);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(nodeToDelete); // put this one on last line so that other can't use deleted one
#endif
        }

        public void UnlinkBothNodes(DialogueNode parentNode, DialogueNode childNode)
        {
            parentNode.RemoveChild(childNode.name);
        }

        public void LinkBothNodes(DialogueNode parentNode, DialogueNode childNode)
        {
            parentNode.AddChild(childNode.name);
        }

        public bool IsBothNodesLinked(DialogueNode parentNode, DialogueNode childNode)
        {
            return parentNode.Children.Contains(childNode.name);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CreateRootNode()
        {
            DialogueNode rootNode = CreateInstance<DialogueNode>();
            rootNode.name = System.Guid.NewGuid().ToString();
            rootNode.Text = "Type First Dialogue script here...";
            rootNode.Speaker = DialogueSpeaker.AI;

            _nodes.Add(rootNode);
            UpdateLookUpTable();
            // IMPORTANT Can't put AddObjectToAsset here bcuz when we call this in first Awake() it won't yet fully create this scriptable object as an asset, So we need to do this when Save the file.
        }

        private void UpdateLookUpTable()
        {
            _nodeLookUpTable.Clear();

            foreach (DialogueNode eachParentNode in _nodes)
            {
                _nodeLookUpTable.Add(eachParentNode.name, eachParentNode);
            }
        }

        private void CleanDanglingNode(DialogueNode nodeToDelete)
        {
            // Remove this node from any of the other node's children list
            foreach (DialogueNode eachNode in _nodes)
            {
                eachNode.RemoveChild(nodeToDelete.name);
            }
        }

        private void UpdateRootNodes()
        {
            _rootNodes = _nodes.ToList(); // Copy without reference

            foreach (DialogueNode eachNodeOutter in _nodes)
            {
                foreach (DialogueNode eachNodeInner in _nodes)
                {
                    if (GetAllChildren(eachNodeOutter).Contains(eachNodeInner))
                        _rootNodes.Remove(eachNodeInner);
                }
            }
        }
        #endregion



        #region --Methods-- (Interface)
        public void OnBeforeSerialize() // Get Called when about to save the file to Hard Drive
        {
#if UNITY_EDITOR
            if (_nodes.Count == 0)
            {
                CreateRootNode();
            }

            // Check whether this Scriptable Object asset has been created already or not, if it's in process of creating it won't yet have a path
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                // Add all the DialogueNode that doesn't yet have an Asset Path
                foreach (DialogueNode eachNode in _nodes)
                {
                    // Check whether eachNode hasn't been created
                    if (AssetDatabase.GetAssetPath(eachNode) == "")
                    {
                        AssetDatabase.AddObjectToAsset(eachNode, this); // Add SubObject to this scriptable object.
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() // Get Called when Load a file from the Hard Drive
        {
        }
        #endregion
    }
}