using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private DialogueSpeaker _speaker;
        [SerializeField] private string _text;
        [SerializeField] private string _questionText;
        [SerializeField] private List<string> _children = new List<string>(); // IF has to initialize first otherwise will get null exception when try to access in GetAllChildren method
        [Tooltip("Width and Height of this is not being used to show ONLY FOR dragging purpose (120 is minimum size from Dialogue.cs), DialogueEditor.cs USE Width and Height from Dialogue.cs scriptable object")]
        [SerializeField] private Rect _rect = new Rect(10, 10, 200, 120);

        [Space]

        [SerializeField] private string _onTriggerEnter;
        [SerializeField] private string _onTriggerExit;
        #endregion



        #region --Properties-- (With Backing Fields)
        public DialogueSpeaker Speaker
        {
            get
            {
                return _speaker;
            }
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Change Dialogue Speaker");
                EditorUtility.SetDirty(this);
#endif
                _speaker = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (Text != value)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Text");
                    EditorUtility.SetDirty(this);
#endif
                    _text = value;
                }
            }
        }

        public string QuestionText
        {
            get
            {
                return _questionText;
            }
            set
            {
                if (QuestionText != value)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Question Text");
                    EditorUtility.SetDirty(this);
#endif
                    _questionText = value;
                }
            }
        }

        public List<string> Children
        {
            get
            {
                return _children;
            }
        }

        public Rect Rect
        {
            get
            {
                return _rect;
            }
        }

        public string OnTriggerEnter
        {
            get
            {
                return _onTriggerEnter;
            }
            set
            {
                if (OnTriggerEnter != value)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Trigger Enter");
                    EditorUtility.SetDirty(this);
#endif
                    _onTriggerEnter = value;
                }
            }
        }

        public string OnTriggerExit
        {
            get
            {
                return _onTriggerExit;
            }
            set
            {
                if (OnTriggerExit != value)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Trigger Exit");
                    EditorUtility.SetDirty(this);
#endif
                    _onTriggerExit = value;
                }
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void AddChild(string childID)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Remove ChildID");
            EditorUtility.SetDirty(this);
#endif
            Children.Add(childID);
        }

        public void RemoveChild(string childID)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Add ChildID");
            EditorUtility.SetDirty(this);
#endif
            Children.Remove(childID);
        }

        public void SetRectPosition(Vector2 newPosition)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Update Dialogue Position");
            EditorUtility.SetDirty(this);
#endif
            _rect.position = newPosition;
        }
        #endregion
    }
}