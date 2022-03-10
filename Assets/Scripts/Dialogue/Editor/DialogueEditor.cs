using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        #region --Fields-- (In Class)
        private Dialogue _selectedDialogue = null;
        private Vector2 _scrollPosition;
        // It get Serialized by Unity as being Editor so need to use this to make it reload this data again.
        [NonSerialized] private GUIStyle _nodeStyle = null;
        [NonSerialized] private DialogueNode _draggingNode = null;
        [NonSerialized] private Vector2 _clickOffSet = new Vector2();
        [NonSerialized] private DialogueNode _creatingNode = null;
        [NonSerialized] private DialogueNode _deletingNode = null;
        [NonSerialized] private DialogueNode _linkingParentNode = null;
        [NonSerialized] private bool _isDraggingCanvas = false;
        [NonSerialized] private Vector2 _draggingCanvasOffset = new Vector2();
        #endregion



        #region --Fields-- (Constant)
        private const float _canvasSize = 2000f;
        private const float _backgroundSize = 50f;
        #endregion



        #region --Methods-- (Annotation)
        [MenuItem("Window/RPG/Dialogue Window", false, 10000)]
        private static void ShowEditorWindow()
        {
            DialogueEditor dialogueEditor = GetWindow(typeof(DialogueEditor), false, "Dialogue") as DialogueEditor;
            dialogueEditor.RefreshDialogueWindow();
        }

        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }
        #endregion



        #region --Methods-- (Built In)
        private void OnEnable()
        {
            Undo.undoRedoPerformed += RepaintWhenUndoRedo;

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _nodeStyle.padding = new RectOffset(15, 15, 15, 15);
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);
            //_nodeStyle.normal.textColor = Color.white; // Not get any effect
        }

        private void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                Rect canvasRect = GUILayoutUtility.GetRect(_canvasSize, _canvasSize); // Create ScrollView Size for BeginScrollView to work
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textureCoords = new Rect(0, 0, _canvasSize / _backgroundSize, _canvasSize / _backgroundSize);
                GUI.DrawTextureWithTexCoords(canvasRect, backgroundTexture, textureCoords);

                // Drawing Bezier Curve first BEFORE Drawing Nodes to make Nodes stay infront of Curves
                foreach (DialogueNode eachNode in _selectedDialogue.Nodes)
                {
                    DrawConnections(eachNode);
                }
                foreach (DialogueNode eachNode in _selectedDialogue.Nodes)
                {
                    DrawNode(eachNode);
                }

                EditorGUILayout.EndScrollView();

                // HAVE TO MODIFY .Nodes list after finish iteration NOT doing inside DrawNode() while it's iterating over .Nodes
                if (_creatingNode != null)
                {
                    _selectedDialogue.CreateChildNodeUnder(_creatingNode);
                    _creatingNode = null;
                }
                if (_deletingNode != null)
                {
                    _selectedDialogue.DeleteThisNode(_deletingNode);
                    _deletingNode = null;
                }
            }
        }

        private void OnSelectionChange()
        {
            RefreshDialogueWindow();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && (_draggingNode == null || _isDraggingCanvas == false)) // && _linkingParentNode == null
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);

                if (_draggingNode != null)
                {
                    _clickOffSet = _draggingNode.Rect.position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _isDraggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                var temp = _draggingNode.Rect;
                temp.position = Event.current.mousePosition + _clickOffSet;
                _draggingNode.Rect = temp;

                Repaint();
            }
            else if (Event.current.type == EventType.MouseDrag && _isDraggingCanvas == true)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;

                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;

                EditorUtility.SetDirty(_selectedDialogue);
            }
            else if (Event.current.type == EventType.MouseUp && _isDraggingCanvas == true)
            {
                _isDraggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            foreach (DialogueNode eachNode in _selectedDialogue.Nodes.Reverse()) // Reverse loop so that it pick the upper layer node first which is that lower bottom of the list
            {
                if (eachNode.Rect.Contains(point))
                {
                    return eachNode;
                }
            }

            
            GUI.FocusControl(null);
            Repaint();
            
            return null;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, _nodeStyle);

            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField($"{node.Text}");

            if (EditorGUI.EndChangeCheck())
            {
                node.Text = newText;

                EditorUtility.SetDirty(_selectedDialogue);
            }

            GUILayout.BeginHorizontal();
            Color defaultGUIColor = GUI.backgroundColor; // Saves the default color

            DrawDeleteButton(node);
            DrawLinkButton(node);
            DrawCreateButton(node);

            GUI.backgroundColor = defaultGUIColor; // Sets the default color back
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);

            foreach (DialogueNode eachChildNode in _selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(eachChildNode.Rect.xMin, eachChildNode.Rect.center.y);

                // Making Curves
                Vector3 curveOffset = endPosition - startPosition; // Make it vaires according to the distance between two nodes
                curveOffset.y = 0; // no need to do curve on Y axis (only X axis)
                curveOffset.x *= 0.8f; // reduce 'curve looks' by 20%

                Handles.DrawBezier(
                    startPosition,
                    endPosition,
                    startPosition + curveOffset,
                    endPosition - curveOffset,
                    Color.white, null, 4f);
            }
        }

        private void RefreshDialogueWindow()
        {
            if (Selection.activeObject is Dialogue newDialogue)
            {
                _selectedDialogue = newDialogue;
                Repaint();
            }
            // CAN'T HAVE else{} here bcuz when we select assign .avtiveObject with something else in ProcessEvents() this else{} will run and close the editor window
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Draw Buttons~
        private void DrawDeleteButton(DialogueNode node)
        {
            GUI.backgroundColor = Color.white; // Set the color for Delete-button
            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }
        }

        private void DrawLinkButton(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                GUI.backgroundColor = Color.white; // Set the color for Link-button
                if (GUILayout.Button("link"))
                {
                    _linkingParentNode = node;
                }
            }
            else if (_linkingParentNode == node)
            {
                GUI.backgroundColor = Color.white; // Set the color for Link-button
                if (GUILayout.Button("CANCEL"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_selectedDialogue.IsBothNodesLinked(_linkingParentNode, node))
            {
                GUI.backgroundColor = Color.red; // Set the color for Link-button
                if (GUILayout.Button("unlink"))
                {
                    _selectedDialogue.UnlinkBothNodes(_linkingParentNode, node);
                    _linkingParentNode = null;
                }
            }
            else
            {
                GUI.backgroundColor = Color.green; // Set the color for Link-button
                if (GUILayout.Button("link here"))
                {
                    _selectedDialogue.LinkBothNodes(_linkingParentNode, node);
                    _linkingParentNode = null;
                }
            }
        }

        private void DrawCreateButton(DialogueNode node)
        {
            if (_linkingParentNode == null)
                GUI.backgroundColor = Color.green; // Set the color for the Add-button
            else
                GUI.backgroundColor = Color.white; // Set the color for the Add-button

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RepaintWhenUndoRedo()
        {
            Repaint();
        }
        #endregion
    }
}