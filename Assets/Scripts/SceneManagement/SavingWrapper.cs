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
        [Header("Transition")]
        [SerializeField] private Transition.Types _transitionType = Transition.Types.Fade;
        [Tooltip("When Start Loading to Other Scene (1 = normal speed)")]
        [SerializeField] private float _startTransitionSpeed = 2f;
        [Tooltip("When End Loading at Other Scene (1 = normal speed)")]
        [SerializeField] private float _endTransitionSpeed = 0.75f;
        #endregion



        #region --Fields-- (In Class)
        private const string _defaultSaveFile = "save";

        public static SavingWrapper Instance { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake() => Instance = this;

        private IEnumerator Start()
        {
            yield return Transition.Instance.StartTransition(_transitionType, _startTransitionSpeed);

            yield return GetComponent<SavingSystem>().LoadLastScene(_defaultSaveFile);

            yield return Transition.Instance.EndTransition(_transitionType, _endTransitionSpeed);
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
        public void Save()
        {
            GetComponent<SavingSystem>().Save(_defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(_defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(_defaultSaveFile);
        }
        #endregion
    }
}