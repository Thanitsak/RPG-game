using UnityEngine;

namespace RPG.Core
{
    public class AudioRandomizer : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private AudioClip[] _audioClips;
        #endregion



        #region --Fields-- (In Class)
        private AudioSource _audioSource;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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