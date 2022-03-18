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
        [SerializeField] private Rect _rectdasdf = new Rect(10, 10, 200, 120);
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
                return _rectdasdf;
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
            _rectdasdf.position = newPosition;
        }
        #endregion
    }
}