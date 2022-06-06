using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.Core
{
    public class Utilities : MonoBehaviour
    {
        #region --Methods-- (Custom PUBLIC) ~For EventSystem Touching~
        public static bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null) return false;

            // the ray cast appears to require only eventData.position
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Rotating~
        /// <summary>
        /// Smooth Rotate Method, use by calling multiple times so it can gradually rotate each time it get called.
        /// There is also the one that return bool indicate whether it is done rotating or not.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        public static void SmoothRotateTo(Transform transform, Transform target, float rotateSpeed)
        {
            // Getting Direction from vector3 by using formula 'targetPos - ourPos'
            Vector3 direction = target.position - transform.position;
            // Get Rotation that we want to rotate to
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
            // Gradually Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        public static void SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smooth Rotate Method, use by calling multiple times so it can gradually rotate each time it get called.
        /// There is also the one that return bool indicate whether it is done rotating or not.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        /// <param name="precisionRate">0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f</param>
        /// <returns>Return False if it's close enough otherwise True.</returns>
        public static bool SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed, float precisionRate)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            return !IsApproximate(transform.rotation, lookRotation, precisionRate);
        }

        public static void SmoothRotateTo(Transform transform, Quaternion targetRotation, float rotateSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Checking Approximation~
        /// <summary>
        /// Checking if Two Quaternions are close or not using precision paramter to define how close.
        /// </summary>
        /// <param name="q1">Quaternion 1st to compare</param>
        /// <param name="q2">Quaternion 2nd to compare</param>
        /// <param name="precision">0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f</param>
        /// <returns>Return True if it's close enough otherwise False.</returns>
        public static bool IsApproximate(Quaternion q1, Quaternion q2, float precision)
        {
            return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
        }
        #endregion
    }
}