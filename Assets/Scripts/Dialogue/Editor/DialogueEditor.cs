using UnityEditor;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        #region --Methods-- (Custom PRIVATE) ~Call Through Editor~
        [MenuItem("Window/RPG/Dialogue Window", false, 10000)]
        private static void InitializeWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue");
        }
        #endregion
    }
}