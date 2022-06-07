using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.SceneManagement;

namespace RPG.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Menu Panel - some buttons directly subscribe to UISwitcher's method")]
        [SerializeField] private Button _continueButton;

        [Header("CreateGame Panel - some buttons directly subscribe to UISwitcher's method")]
        [SerializeField] private Button _createNewGameButton;
        [SerializeField] private TMP_InputField _newGameInputField;
        #endregion



        #region --Fields-- (In Class)
        private string _inputText;
        #endregion



        #region --Methods-- (Built In)
        private void Start() // Can't use Awake() since SavingWrapper.Instance is not initialize yet, equals to null
        {
            _continueButton.gameObject.SetActive(SavingWrapper.Instance.CurrentSaveFileExists());

            _continueButton.onClick.AddListener(ContinueGame);

            _newGameInputField.onEndEdit.AddListener(UpdateInputText); // .onEndEdit take subscriber method with Parameter as string
            _createNewGameButton.onClick.AddListener(StartNewGame);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void ContinueGame()
        {
            SavingWrapper.Instance.ContinueGame();
        }

        private void QuitGame()
        {

        }

        private void UpdateInputText(string inputText)
        {
            _inputText = inputText.Trim();
        }

        private void StartNewGame()
        {
            if (string.IsNullOrEmpty(_inputText))
            {
                Debug.LogError("Plese Enter Save File Name First!");
                return;
            }

            SavingWrapper.Instance.StartNewGame(_inputText);
        }
        #endregion
    }
}