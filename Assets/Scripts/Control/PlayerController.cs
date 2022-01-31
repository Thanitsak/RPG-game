using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _maxNavMeshDetectionRange = 2f;
        [SerializeField] private CursorMapping[] _cursorMappings = null;
        #endregion



        #region --Fields-- (In Class)
        private Camera _camera;
        private Mover _mover;
        private Health _health;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _camera = Camera.main;
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI()) return;
            if (_health.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Raycast Stuff~
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hitsInfo = RaycastAllSorted();

            foreach (RaycastHit eachHit in hitsInfo)
            {
                foreach (IRaycastable eachRaycastable in eachHit.transform.GetComponents<IRaycastable>())
                {
                    if (eachRaycastable.HandleRaycast(this))
                    {
                        SetCursor(eachRaycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            if (RaycastNavMesh(out Vector3 target))
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target, 1f);
                }

                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            // Raycast to Terrain
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo))
            {
                // Find Nearest NavMeshPoint
                if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navMeshHit, _maxNavMeshDetectionRange, NavMesh.AllAreas))
                {
                    target = navMeshHit.position;
                    return true;
                }
            }

            target = Vector3.zero;
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            // Draw the ray GET ALL, WON'T GET BLOCK & SORTED with Hit Distance
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            float[] distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private Ray GetMouseRay() => _camera.ScreenPointToRay(Input.mousePosition); // get ray direction from camera to a screen point
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Cursor Stuff~
        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping eachMapping in _cursorMappings)
            {
                if (eachMapping.cursorType == cursorType)
                {
                    return eachMapping;
                }
            }

            return _cursorMappings[0];
        }
        #endregion



        #region --Structs-- (Custom PRIVATE)
        [System.Serializable]
        private struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        #endregion
    }
}