using System.Collections;
using UnityEngine;
using RPG.Saving;
using RPG.Utils;

namespace RPG.SceneManagement
{
    /// <summary>
    /// This component provides the methods to save and load a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("LoadLastScene Transition Settings")]
        [SerializeField] private Transition.Types _llsTransitionType = Transition.Types.Fade;
        [Tooltip("When Start Loading to Other Scene (1 = normal speed)")]
        [SerializeField] private float _llsTtartTransitionSpeed = 2f;
        [Tooltip("When End Loading at Other Scene (1 = normal speed)")]
        [SerializeField] private float _llsEndTransitionSpeed = 0.75f;

        [Header("LoadFirstScene Transition Settings")]
        [SerializeField] private int _lfsBuildIndexToLoad = 1;
        #endregion



        #region --Fields-- (In Class)
        private AutoInit<SavingSystem> _savingSystem;

        public static SavingWrapper Instance { get; private set; }
        #endregion



        #region --Fields-- (Constant)
        private const string _currentSaveKey = "CurrentSaveKey";
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Instance = this;

            _savingSystem = new AutoInit<SavingSystem>(() => GetComponent<SavingSystem>()); // Use AutoInit so that when other classes use public methods in their Start() SavingSystem won't be null
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(_currentSaveKey)) return; // Don't Have current save Key yet!
            if (!CurrentSaveFileExists()) return; // Actual SaveFile is NOT Exists!

            StartCoroutine(LoadLastSceneWithTransition());
        }
        public IEnumerator LoadLastSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);
            yield return _savingSystem.value.LoadLastScene(GetCurrentSaveName());
            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }

        public void StartNewGame(string saveFileName)
        {
            if (string.IsNullOrEmpty(saveFileName)) return;

            SetCurrentSaveName(saveFileName);
            StartCoroutine(LoadFirstSceneWithTransition());
        }
        public IEnumerator LoadFirstSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);
            yield return Transition.Instance.LoadAsynchronously(_lfsBuildIndexToLoad);
            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }

        public void Save() => _savingSystem.value.Save(GetCurrentSaveName());

        public void Load() => _savingSystem.value.Load(GetCurrentSaveName()); 

        public void Delete() => _savingSystem.value.Delete(GetCurrentSaveName());

        public bool CurrentSaveFileExists() => _savingSystem.value.SaveFileExists(GetCurrentSaveName());
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void SetCurrentSaveName(string currentSaveName)
        {
            PlayerPrefs.SetString(_currentSaveKey, currentSaveName);
        }

        private string GetCurrentSaveName()
        {
            return PlayerPrefs.GetString(_currentSaveKey);
        }
        #endregion
    }
}