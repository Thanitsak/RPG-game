using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _waypointRadius = 1f;
        [SerializeField] private float _waypointLineWidth = 2f;
        #endregion



        #region --Methods-- (Built In)
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWaypoint(i), _waypointRadius);
                DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)), _waypointLineWidth);

                Gizmos.color = Color.white;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public int GetNextIndex(int index) => (index == transform.childCount - 1) ? 0 : index + 1;

        public Vector3 GetWaypoint(int index) => transform.GetChild(index).position;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void DrawLine(Vector3 p1, Vector3 p2, float width)
        {
            int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
            if (count == 1)
            {
                Gizmos.DrawLine(p1, p2);
            }
            else
            {
                Camera c = Camera.current;
                if (c == null)
                {
                    Debug.LogError("Camera.current is null");
                    return;
                }
                var scp1 = c.WorldToScreenPoint(p1);
                var scp2 = c.WorldToScreenPoint(p2);

                Vector3 v1 = (scp2 - scp1).normalized; // line direction
                Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector

                for (int i = 0; i < count; i++)
                {
                    Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                    Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                    Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                    Gizmos.DrawLine(origin, destiny);
                }
            }
        }
        #endregion
    }
}