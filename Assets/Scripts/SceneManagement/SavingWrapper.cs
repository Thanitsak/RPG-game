using System.Collections;
using UnityEngine;
using RPG.Saving;

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
        #endregion



        #region --Fields-- (In Class)
        private const string _defaultSaveFile = "save";
        private SavingSystem _savingSystem;

        public static SavingWrapper Instance { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake() => Instance = this;

        private void Start()
        {
            _savingSystem = GetComponent<SavingSystem>();
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
            StartCoroutine( LoadLastSceneWithTransition() );
        }

        public void Save()
        {
            _savingSystem.Save(_defaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(_defaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(_defaultSaveFile);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator LoadLastSceneWithTransition()
        {
            yield return Transition.Instance.StartTransition(_llsTransitionType, _llsTtartTransitionSpeed);

            yield return _savingSystem.LoadLastScene(_defaultSaveFile);

            yield return Transition.Instance.EndTransition(_llsTransitionType, _llsEndTransitionSpeed);
        }
        #endregion
    }
}