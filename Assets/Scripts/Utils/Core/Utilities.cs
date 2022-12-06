using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.Core
{
    public static class Utilities
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
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        public static void SmoothRotateTo(Transform transform, Transform target, float rotateSpeed)
        {
            Vector3 direction = target.position - transform.position; // Getting Direction from vector3 by using formula 'targetPos - ourPos'
            Quaternion lookRotation = LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up); // Get Rotation that we want to rotate to
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime); // Gradually Rotate
        }

        public static void SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed)
        {
            Quaternion lookRotation = LookRotation(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        public static void SmoothRotateTo(Transform transform, Quaternion targetRotation, float rotateSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smooth Rotate Method, use by calling multiple times so it can gradually rotate each time it get called.
        /// There is also the one that return bool indicate whether it is done rotating or not.
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        /// <param name="precisionRate">0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f</param>
        /// <returns>Return False if it's close enough otherwise True.</returns>
        public static bool SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed, float precisionRate)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            return !IsApproximate(transform.rotation, lookRotation, precisionRate);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Checking Approximation~
        /// <summary>
        /// Checking if Two Quaternions are close or not using precision paramter to define how close.
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
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



        #region --Methods-- (Custom PUBLIC) ~Improvement~
        /// <summary>
        /// Fix 'Look rotation viewing vector is zero' message in console.
        /// Problem Come from Quaternion.LookRotation() bacause it is impossible to find any angle between two points when those points are the same as being zero.
        /// Use this method instead of calling Quaternion.LookRotation().
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
        {
            return forward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Both Arguments position1 and position2 can be swap without affecting the distance result. This is faster than using .Distance() or .magnitude
        /// This return Square Distance, to compare with Actual Distance simply do (Actual Distance * Actual Distance) to make it comparable. This gives a better performance.
        /// For more details, check 'Vector3 Code' section in 'Unity Doc' note.
        /// </summary>
        public static float SqrDistance(Vector3 position1, Vector3 position2)
        {
            Vector3 offset = position1 - position2;
            return offset.sqrMagnitude;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Animation~
        /// <summary>
        /// To get NormalizedTime of the playing Animation State, also works when transitioning between Animation States.
        /// For more details, check 'Animation Code' section in 'Unity Doc' note.
        /// </summary>
        /// <returns>NormalizedTime in range of [0f, Infinity]</returns>
        public static float GetNormalizedTime(Animator animator)
        {
            // We can't simply return currentState.normalizedTime while There is Transitioning between States
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);

            // While Transitioning we have to get normalizedTime from nextState
            if (animator.IsInTransition(0) && nextState.IsTag("Attack"))
            {
                return nextState.normalizedTime;
            }
            // While Transitioning is STOP we have to get normalizedTime from currentState
            else if (!animator.IsInTransition(0) && currentState.IsTag("Attack"))
            {
                return currentState.normalizedTime;
            }
            // Incase something is goes wrong
            else
            {
                return 0f;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Detecting~
        /// <summary>
        /// To check if a specify target is on the Player Screen Display or not.
        /// For more details, check 'Camera Methods Tips' section in 'Unity Doc' note.
        /// </summary>
        public static bool IsOnScreen(Vector3 target, Camera mainCamera)
        {
            Vector2 viewPosition = mainCamera.WorldToViewportPoint(target);
            return viewPosition.x >= 0f && viewPosition.x <= 1f && viewPosition.y >= 0f && viewPosition.y <= 1f;
        }
        #endregion
    }
}