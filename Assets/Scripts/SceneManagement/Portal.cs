using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }



        #region --Fields-- (Inspector)
        [SerializeField] private int _sceneIndexToLoad = -1;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIdentifier _destination;

        [Header("Transition")]
        [SerializeField] private Transition.Types _transitionType = Transition.Types.CircleWipe;
        [Tooltip("When Start Loading to Other Scene (1 = normal speed)")]
        [SerializeField] private float _startTransitionSpeed = 1f;
        [Tooltip("When End Loading at Other Scene (1 = normal speed)")]
        [SerializeField] private float _endTransitionSpeed = 1f;
        #endregion



        #region --Methods-- (Built In)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(LoadLevelWithTransition());
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Coroutines~
        private IEnumerator LoadLevelWithTransition()
        {
            if (_sceneIndexToLoad < 0)
            {
                Debug.LogError("Please Set SceneIndexToLoad First!");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            PlayerController playerOnOldScene = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerController>();
            playerOnOldScene.enabled = false;

            // Wait until StartTransition DONE
            yield return Transition.Instance.StartTransition(_transitionType, _startTransitionSpeed);

            SavingWrapper.Instance.Save();

            // Wait until LoadScene DONE
            yield return Transition.Instance.LoadAsynchronously(_sceneIndexToLoad);

            PlayerController playerOnNEWScene = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerController>();
            playerOnNEWScene.enabled = false;

            SavingWrapper.Instance.LoadCurrentSave();
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            SavingWrapper.Instance.Save();

            // DON'T Wait for EndTransition, so player won't be freeze BUT we can't enable it before this CUZ player might walk into portal again
            Transition.Instance.EndTransition(_transitionType, _endTransitionSpeed);

            playerOnNEWScene.enabled = true;

            Destroy(gameObject);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Utilities~
        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();

            foreach (Portal each in portals)
                if (each != this && each._destination == _destination)
                    return each;

            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal._spawnPoint.position); // No Conflict with NavMeshAgent by setting position this way
            player.transform.rotation = otherPortal._spawnPoint.rotation;
        }
        #endregion
    }
}