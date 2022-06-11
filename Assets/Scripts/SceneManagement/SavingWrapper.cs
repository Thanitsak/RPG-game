using System.Collections;
using System.Collections.Generic;
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

        [Header("LoadMenuScene Transition Settings")]
        [SerializeField] private int _lmsBuildIndexToLoad = 0;

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

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Save();
        //    }

        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        LoadCurrentSave();
        //    }

        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        DeleteCurrentSave();
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(_currentSaveKey)) return; // Don't Have current save Key yet!
            if (!CurrentSaveFileExists()) return; // Actual SaveFile is NOT Exists!

            StartCoroutine(LoadLastSceneWithTransition());
        }

        public void StartNewGame(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            SetCurrentSaveName(fileName);
            StartCoroutine(LoadFirstSceneWithTransition());
        }

        public void LoadFromSaveFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            SetCurrentSaveName(fileName);
            ContinueGame();
        }

        public void LoadMenuScene()
        {
            StartCoroutine(LoadMenuSceneWithTransition());
        }

        public void Save() => _savingSystem.value.Save(GetCurrentSaveName());

        public void LoadCurrentSave() => _savingSystem.value.Load(GetCurrentSaveName()); 

        public void DeleteCurrentSave() => _savingSystem.value.Delete(GetCurrentSaveName());

        public bool CurrentSaveFileExists() => _savingSystem.value.SaveFileExists(GetCurrentSaveName());

        public IEnumerable<string> ListSaves() => _savingSystem.value.ListSaves();
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Loading With Transition~
        private IEnumerator LoadLastSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);
            yield return _savingSystem.value.LoadLastScene(GetCurrentSaveName());
            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }

        private IEnumerator LoadFirstSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);
            yield return Transition.Instance.LoadAsynchronously(_lfsBuildIndexToLoad);
            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }

        private IEnumerator LoadMenuSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);
            yield return Transition.Instance.LoadAsynchronously(_lmsBuildIndexToLoad);
            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~PlayerPrefs Saving~
        // **Current Save is mainly for Continue Game to work (so it knows what save file is currently used)**
        private void SetCurrentSaveName(string currentFileName)
        {
            PlayerPrefs.SetString(_currentSaveKey, currentFileName);
        }

        private string GetCurrentSaveName()
        {
            return PlayerPrefs.GetString(_currentSaveKey);
        }
        #endregion
    }
}