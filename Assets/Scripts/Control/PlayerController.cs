using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Attributes;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Tooltip("Only Navigate where there is NavMesh with 'extended range' to place where there is No NavMesh")]
        [Range(0.1f, 4f)]
        [SerializeField] private float _maxNavMeshDetectionRange = 2f;
        [Tooltip("How long can Player Travel? FOR Cursor Detection otherwise player can just Click in one shot on the other side of river for example.")]
        [SerializeField] private float _maxDestinationLength = 40f;
        [SerializeField] private CursorMapping[] _cursorMappings = null;
        [SerializeField] private float _raycastRadius = 0.5f;
        #endregion



        #region --Fields-- (In Class)
        private Camera _camera;
        private Mover _mover;
        private Health _health;

        private bool _isDraggingUI = false;
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
            if (Input.GetMouseButtonUp(0))
            {
                _isDraggingUI = false;
            }

            if (Utilities.IsPointerOverUIObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isDraggingUI = true;
                }

                SetCursor(CursorType.UI);
                return true;
            }

            if (_isDraggingUI)
            {
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
                if (!_mover.CanMoveToUnderMaxLength(target, _maxDestinationLength)) return false;

                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target, 1f);
                }

                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            // Draw the ray GET ALL, WON'T GET BLOCK & SORTED with Hit Distance
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);

            float[] distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            // Raycast to Terrain
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo))
            {
                // Only Navigate where there is NavMesh
                if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navMeshHit, _maxNavMeshDetectionRange, NavMesh.AllAreas))
                {
                    target = navMeshHit.position;
                    return true;
                }
            }

            target = Vector3.zero;
            return false;
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