using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicControlRemover : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;
        private PlayerController _playerController;

        private PlayableDirector _playableDirector;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _actionScheduler = GameObject.FindWithTag("Player").GetComponent<ActionScheduler>();
            _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            _playableDirector = GetComponent<PlayableDirector>();

            _playableDirector.played += DisableControl;
            _playableDirector.stopped += EnableControl;
        }
        #endregion



        #region --Methods-- (Custom)
        private void DisableControl(PlayableDirector playableDirector)
        {
            _actionScheduler.StopCurrentAction();

            _playerController.enabled = false;
        }

        private void EnableControl(PlayableDirector playableDirector)
        {
            _playerController.enabled = true;
        }
        #endregion
    }
}