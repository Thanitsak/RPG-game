using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using RPG.Control;
using RPG.Core;

namespace RPG.SceneManagement
{
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
        public IEnumerator StartTransition(Types types, float transitioningSpeed)
        {
            // Disable PlayerControl
            GameObject.FindWithTag("Player").GetComponent<ActionScheduler>().StopCurrentAction();

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

            _animators[index].speed = transitioningSpeed;
            _animators[index].Play("Transition Start", -1, 0f);

            // While Animation Clip is still the OLD one, wait for it to get update
            while (!_animators[index].GetCurrentAnimatorStateInfo(0).IsName("Transition Start"))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (_animators[index].GetCurrentAnimatorStateInfo(0).IsName("Transition Start") &&
                _animators[index].GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        public IEnumerator EndTransition(Types types, float transitioningSpeed)
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

            _animators[index].speed = transitioningSpeed;
            _animators[index].Play("Transition End", -1, 0f);

            // While Animation Clip is still the OLD one, wait for it to get update
            while (!_animators[index].GetCurrentAnimatorStateInfo(0).IsName("Transition End"))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (_animators[index].GetCurrentAnimatorStateInfo(0).IsName("Transition End") &&
                _animators[index].GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
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



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator OpenLoadingScreen()
        {
            _loadingScreenAnimator.Play("LoadingScreen Start", -1, 0f);

            // While Animation Clip is still the OLD one, wait for it to get update
            while (!_loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("LoadingScreen Start"))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (_loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("LoadingScreen Start") &&
                _loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        private IEnumerator CloseLoadingScreen()
        {
            _loadingScreenAnimator.Play("LoadingScreen End", -1, 0f);

            // While Animation Clip is still the OLD one, wait for it to get update
            while (!_loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("LoadingScreen End"))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (_loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("LoadingScreen End") &&
                _loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
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