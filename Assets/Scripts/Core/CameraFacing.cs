using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Camera _mainCamera;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = _mainCamera.transform.forward;

            // OLD Code Formula to Rotation
            //float result = _mainCamera.transform.eulerAngles.y + (-transform.parent.eulerAngles.y);
            //transform.localEulerAngles = new Vector3(0f, result, 0f);
        }
        #endregion
    }
}