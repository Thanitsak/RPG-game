using UnityEngine;

namespace RPG.UI.Temp
{
    public class ShowHideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] GameObject uiContainer = null;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            uiContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                uiContainer.SetActive(!uiContainer.activeSelf);
            }
        }
        #endregion
    }
}