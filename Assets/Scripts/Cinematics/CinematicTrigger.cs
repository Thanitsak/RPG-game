using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicTrigger : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private bool _isTriggered = false;

        private PlayableDirector _playableDirector;
        #endregion



        #region --Methods-- (Build In)
        private void Start() => _playableDirector = GetComponent<PlayableDirector>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                _playableDirector.Play();
                _isTriggered = true;
            }
        }
        #endregion
    }
}