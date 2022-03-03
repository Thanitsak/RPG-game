using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        #region --Fields-- (In Class)
        private Dialogue _selectedDialogue = null;
        private GUIStyle _nodeStyle = null;
        private bool _isDragging = false;
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

                foreach (DialogueNode eachNode in _selectedDialogue.Nodes)
                {
                    OnGUINode(eachNode);
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
            if (Event.current.type == EventType.MouseDown && !_isDragging)
            {
                _isDragging = true;
            }
            else if (Event.current.type == EventType.MouseDrag && _isDragging)
            {
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Position");

                _selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _isDragging)
            {
                _isDragging = false;
            }
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, _nodeStyle);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node : ", EditorStyles.whiteLargeLabel);
            string newID = EditorGUILayout.TextField($"{node.uniqueID}");
            string newText = EditorGUILayout.TextField($"{node.text}");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Field"); // This will undo only one at a time because once changes is made on any of the field this will trigger right away

                node.text = newText;
                node.uniqueID = newID;

                EditorUtility.SetDirty(_selectedDialogue);
            }

            GUILayout.EndArea();
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
    }
}