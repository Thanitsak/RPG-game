using UnityEngine;

namespace RPG.Core
{
    public class AudioRandomizer : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private bool _playOnAwake = false;
        #endregion



        #region --Fields-- (In Class)
        private AudioSource _audioSource;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (_playOnAwake)
                PlayRandomClip();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void PlayRandomClip()
        {
            int randClipIndex = Random.Range(0, _audioClips.Length);
            _audioSource.PlayOneShot(_audioClips[randClipIndex]);
        }
        #endregion
    }
}