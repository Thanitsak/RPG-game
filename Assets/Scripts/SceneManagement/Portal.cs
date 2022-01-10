using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private int _sceneIndexToLoad = -1;
        [SerializeField] private Transform spawnPoint;
        #endregion



        #region --Methods-- (Built In)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            yield return SceneManager.LoadSceneAsync(_sceneIndexToLoad);

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();

            foreach (Portal each in portals)
                if (each != this)
                    return each;

            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); // No Conflict with NavMeshAgent by setting position this way
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }
        #endregion
    }
}