using UnityEngine;

namespace RPG.UI.Temp
{
    public class ShowHideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] private GameObject _uiContainer = null;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _uiContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                _uiContainer.SetActive(!_uiContainer.activeSelf);
            }
        }
        #endregion
    }
}