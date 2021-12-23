using UnityEngine;

namespace RPG.Core
{
    public class Utilities : MonoBehaviour
    {
        #region --Methods-- (Custom PUBLIC)
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