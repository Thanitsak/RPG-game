using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Untitled Delayed Click Targeting", menuName = "RPG/Game Item/Targeting/New Delayed Click", order = 126)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Texture2D _cursorTexture;
        [SerializeField] private Vector2 _cursorHotspot;

        [Space]

        [Tooltip("Area Range for this Affect")]
        [SerializeField] private float _areaAffectRadius = 4f;
        [Tooltip("How far can this Affect Start")]
        [SerializeField] private float _availableDistance = 500f;
        [Tooltip("Layer that the Affect will start on (Usually only on Terrain layer)")]
        [SerializeField] private LayerMask _affectStarterLayer;
        [Tooltip("Prefab for indicating affect area on the ground.")]
        [SerializeField] private GameObject _targetingPrefab;
        #endregion



        #region --Fields-- (In Class)
        private GameObject _targetingPrefabInstance = null;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action onFinished)
        {
            playerController.enabled = false;

            if (_targetingPrefabInstance == null)
                _targetingPrefabInstance = Instantiate(_targetingPrefab);
            else
                _targetingPrefabInstance.SetActive(true);

            _targetingPrefabInstance.transform.localScale = new Vector3(_areaAffectRadius * 2f, 1f, _areaAffectRadius * 2f); // *2 because scale needs diameter value not just radius.

            while (true)
            {
                Cursor.SetCursor(_cursorTexture, _cursorHotspot, CursorMode.Auto);

                // Cast a ray from mouse to the Ground ONLY
                if (Physics.Raycast(playerController.GetTouchRay(), out RaycastHit hit, _availableDistance, _affectStarterLayer))
                {
                    _targetingPrefabInstance.transform.position = hit.point;

                    if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
                    {
                        yield return new WaitWhile(() => Input.GetMouseButton(0)); // This will return as clicked for couple of frames, so wait until mouse is up.

                        playerController.ResetCursorType();
                        playerController.enabled = true; // If enable while mouse is down, InteractWithMovement will triggered

                        data.TargetedPoint = hit.point;
                        data.Targets = GetGameObjectsInRadius(hit.point);
                        onFinished?.Invoke();

                        _targetingPrefabInstance.SetActive(false);

                        yield break;
                    }
                }
                else
                {
                    _targetingPrefabInstance.SetActive(false);
                    // TODO something to indicate player that can't cast ability
                    Debug.LogWarning($"Can only cast on Ground and has to be in range of {_availableDistance}. CAN DO SOMETHING TO INDICATE PLAYER HERE");
                }

                yield return null;
            }
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            // do SphereCast from the point of ray that hit the Ground.
            RaycastHit[] hits = Physics.SphereCastAll(point, _areaAffectRadius, Vector3.up, 0f);

            foreach (RaycastHit hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
        #endregion



        #region --Methods-- (Override)
        public override void StartTargeting(AbilityData data, Action onFinished)
        {
            PlayerController playerController = data.User.transform.root.GetComponentInChildren<PlayerController>();
            data.StartCoroutine( Targeting(data, playerController, onFinished) );
        }
        #endregion
    }
}