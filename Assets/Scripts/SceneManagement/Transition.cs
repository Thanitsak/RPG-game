using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace RPG.SceneManagement
{
    /// <summary>
    /// This component provides the methods to load scene with transitions also Asynchronously.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class Transition : MonoBehaviour
    {
        public enum Types
        {
            Fade,
            CircleWipe
        }



        #region --Fields-- (Inspector)
        [Header("Level Transitions")]
        [SerializeField] private Animator[] _animators;

        [Header("Loading Screen")]
        [SerializeField] private Animator _loadingScreenAnimator;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Slider _loadingBar;
        #endregion



        #region --Fields-- (In Class)
        private const float _unityLoadingStateMax = 0.9f;

        public static Transition Instance { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake() => Instance = this;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Transitions~
        public Coroutine StartTransition(Types types, float transitioningSpeed)
        {
            _animators[GetIndex(types)].speed = transitioningSpeed;
            _animators[GetIndex(types)].Play("Transition Start", -1, 0f);

            return StartCoroutine(WaitForClipFinished(_animators[GetIndex(types)], "Transition Start"));
        }

        public Coroutine EndTransition(Types types, float transitioningSpeed)
        {
            _animators[GetIndex(types)].speed = transitioningSpeed;
            _animators[GetIndex(types)].Play("Transition End", -1, 0f);

            return StartCoroutine(WaitForClipFinished(_animators[GetIndex(types)], "Transition End"));
        }

        public IEnumerator LoadAsynchronously(int sceneIndexToLoad)
        {
            // ResetLoadingBar progress first SO when it open up with animation value start from 0
            ResetLoadingBar();
            yield return OpenLoadingScreen();

            // Start Loading Scene
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndexToLoad);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / _unityLoadingStateMax);

                UpdateLoadingBar(progress);

                yield return null;
            }

            yield return CloseLoadingScreen();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~General~
        private IEnumerator WaitForClipFinished(Animator animator, string clipName)
        {
            // While Animation Clip is still the OLD one, wait for it to get update
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        private int GetIndex(Types types)
        {
            int index = 0;

            switch (types)
            {
                case Types.Fade:
                    index = 0;
                    break;

                case Types.CircleWipe:
                    index = 1;
                    break;
            }

            return index;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Loading Screen~
        private IEnumerator OpenLoadingScreen()
        {
            _loadingScreenAnimator.Play("LoadingScreen Start", -1, 0f);

            yield return StartCoroutine(WaitForClipFinished(_loadingScreenAnimator, "LoadingScreen Start"));
        }

        private IEnumerator CloseLoadingScreen()
        {
            _loadingScreenAnimator.Play("LoadingScreen End", -1, 0f);

            yield return StartCoroutine(WaitForClipFinished(_loadingScreenAnimator, "LoadingScreen End"));
        }

        private void UpdateLoadingBar(float progress)
        {
            _loadingBar.value = progress;
            _progressText.text = $"{(progress * 100f):N0}%";
        }

        private void ResetLoadingBar()
        {
            _loadingBar.value = 0f;
            _progressText.text = $"{0f}%";
        }
        #endregion
    }
}