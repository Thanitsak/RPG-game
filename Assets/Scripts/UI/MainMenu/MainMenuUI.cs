using UnityEngine;
using UnityEngine.UI;
using RPG.SceneManagement;

namespace RPG.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Button _continueButton;
        #endregion



        #region --Methods-- (Built In)
        private void Start() // Can't use Awake() since SavingWrapper.Instance is not initialize yet, equals to null
        {
            _continueButton.onClick.AddListener(SavingWrapper.Instance.ContinueGame);
        }
        #endregion
    }
}