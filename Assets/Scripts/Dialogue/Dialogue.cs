using UnityEngine;

[CreateAssetMenu(fileName = "Unnamed Dialogue", menuName = "RPG/Dialogue/New Dialogue")]
public class Dialogue : ScriptableObject
{
    #region --Fields-- (Inspector)
    [SerializeField] private DialogueNode[] _nodes = null;
    #endregion
}