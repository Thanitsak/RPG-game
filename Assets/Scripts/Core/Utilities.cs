using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Core
{
    public class Utilities : MonoBehaviour
    {
        #region --Methods-- (Custom PUBLIC) ~For EventSystem Touching~
        public static bool IsPointerOverUIObject()
        {
            // the ray cast appears to require only eventData.position
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Rotating~
        public static void SmoothRotateTo(Transform transform, Transform target, float rotateSpeed)
        {
            // Getting Direction from vector3 by using formula 'targetPos - ourPos'
            Vector3 direction = target.position - transform.position;

            // Get Rotation that we want to rotate to
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);

            // Gradually Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        public static void SmoothRotateTo(Transform transform, Quaternion targetRotation, float rotateSpeed)
        {
            // Gradually Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        #endregion
    }
}