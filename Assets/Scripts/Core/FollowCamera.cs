using UnityEngine;

namespace RPGgame
{
    public class FollowCamera : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Follow Camera Properties")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offSet;
        [SerializeField] private float _smoothSpeed = 0.2f;
        #endregion



        #region --Fields-- (In Class)
        private Vector3 _smoothVelocityRef = Vector3.zero; // Don't have to use just for Reference
        #endregion



        #region --Methods-- (Built In)
        private void LateUpdate()
        {
            Vector3 targetPostion = _target.position + _offSet;

            transform.position = Vector3.SmoothDamp(transform.position, targetPostion, ref _smoothVelocityRef, _smoothSpeed);
        }
        #endregion
    }
}