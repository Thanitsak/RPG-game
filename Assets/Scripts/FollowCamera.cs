using UnityEngine;

namespace RPGgame
{
    public class FollowCamera : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Transform _target;
        #endregion



        #region --Methods-- (Built In)
        private void Update()
        {
            transform.position = _target.position;
        }
        #endregion
    }
}