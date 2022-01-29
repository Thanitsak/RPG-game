using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        #region --Fields-- (In Class)
        private bool _isTriggered = false;

        private PlayableDirector _playableDirector;
        #endregion



        #region --Methods-- (Build In)
        private void Awake() => _playableDirector = GetComponent<PlayableDirector>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                _playableDirector.Play();
                _isTriggered = true;
            }
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return _isTriggered;
        }

        void ISaveable.RestoreState(object state)
        {
            _isTriggered = (bool)state;
        }
        #endregion
    }
}