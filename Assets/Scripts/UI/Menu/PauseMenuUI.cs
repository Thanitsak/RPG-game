using UnityEngine;
using UnityEngine.UI;
using RPG.Control;
using RPG.SceneManagement;

namespace RPG.UI.Menu
{
    /// <summary>
    /// This should be placed on the GameObject that will be Active & Deactivate since there is OnEnabled() method
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Pause Panel - some button is directly subscribed to public method")]
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _saveAndQuitButton;
        #endregion



        #region --Fields-- (In Class)
        private PlayerController _playerController;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerController = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerController>();

            _saveButton.onClick.AddListener(Save);
            _saveAndQuitButton.onClick.AddListener(SaveAndQuit);
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



        #region --Methods-- (Subscriber)
        private void Save()
        {
            SavingWrapper.Instance.Save();
        }

        private void SaveAndQuit()
        {
            SavingWrapper.Instance.Save();
            SavingWrapper.Instance.LoadMenuScene();
        }
        #endregion
    }
}