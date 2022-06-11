using UnityEngine;
using RPG.Control;

namespace RPG.UI.Menu
{
    /// <summary>
    /// This should be placed on the GameObject that will be Active & Deactivate since there is OnEnabled() method
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private PlayerController _playerController;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerController = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerController>();
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
            _playerController.enabled = false;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            _playerController.enabled = true;
        }
        #endregion
    }
}