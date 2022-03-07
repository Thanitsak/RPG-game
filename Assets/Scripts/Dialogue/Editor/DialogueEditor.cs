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
        // It get Serialized by Unity as being Editor so need to use this to make it reload this data again.
        [NonSerialized] private GUIStyle _nodeStyle = null;
        [NonSerialized] private DialogueNode _draggingNode = null;
        [NonSerialized] private Vector2 _clickOffSet = new Vector2();
        [NonSerialized] private DialogueNode _creatingNode = null;
        [NonSerialized] private DialogueNode _deletingNode = null;
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

                // Drawing Bezier Curve first BEFORE Drawing Nodes to make Nodes stay infront of Curves
                foreach (DialogueNode eachNode in _selectedDialogue.Nodes)
                {
                    DrawConnections(eachNode);
                }
                foreach (DialogueNode eachNode in _selectedDialogue.Nodes)
                {
                    DrawNode(eachNode);
                }
                // HAVE TO MODIFY .Nodes list after finish iteration NOT doing inside DrawNode() while it's iterating over .Nodes
                if (_creatingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");

                    _selectedDialogue.CreateChildNode(_creatingNode);
                    _creatingNode = null;
                }
                if (_deletingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Deleted Dialogue Node");

                    _selectedDialogue.DeleteItselfNode(_deletingNode);
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
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);

                if (_draggingNode != null)
                    _clickOffSet = _draggingNode.Rect.position - Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Position");

                var temp = _draggingNode.Rect;
                temp.position = Event.current.mousePosition + _clickOffSet;
                _draggingNode.Rect = temp;

                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;

                EditorUtility.SetDirty(_selectedDialogue);
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
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Field");

                node.Text = newText;

                EditorUtility.SetDirty(_selectedDialogue);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }
            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
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
            else
            {
                _selectedDialogue = null;
                Repaint();
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