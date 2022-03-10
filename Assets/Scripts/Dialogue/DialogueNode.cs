using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new List<string>(); // IF has to initialize first otherwise will get null exception when try to access in GetAllChildren method
        [SerializeField] private Rect _rect = new Rect(10, 10, 200, 100);
        #endregion



        #region --Properties-- (With Backing Fields)
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialogue Field");
#endif
                _text = value;
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
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialogue Position");
#endif
                _rect = value;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void AddChild(string childID)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Remove ChildID");
#endif
            Children.Add(childID);
        }

        public void RemoveChild(string childID)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Add ChildID");
#endif
            Children.Remove(childID);
        }
        #endregion
    }
}