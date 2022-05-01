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
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator Targeting(GameObject user, PlayerController playerController, Action<IEnumerable<GameObject>> onFinished)
        {
            playerController.enabled = false;

            while (true)
            {
                Cursor.SetCursor(_cursorTexture, _cursorHotspot, CursorMode.Auto);

                if (Input.GetMouseButtonDown(0))
                {
                    // This will return as clicked for couple of frames, so wait until mouse is up.
                    yield return new WaitWhile(() => Input.GetMouseButton(0));

                    playerController.ResetCursorType();
                    // If enable while mouse is down, InteractWithMovement will triggered
                    playerController.enabled = true;

                    onFinished?.Invoke( GetGameObjectsInRadius(playerController) );

                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(PlayerController playerController)
        {
            // Cast a ray from mouse to the Ground ONLY, then do SphereCast from there.
            if (Physics.Raycast(playerController.GetMouseRay(), out RaycastHit raycastHit, _availableDistance, _affectStarterLayer))
            {
                RaycastHit[] hits = Physics.SphereCastAll(raycastHit.point, _areaAffectRadius, Vector3.up, 0f);

                foreach (RaycastHit hit in hits)
                {
                    yield return hit.collider.gameObject;
                }
            }
        }
        #endregion



        #region --Methods-- (Override)
        public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> onFinished)
        {
            PlayerController playerController = user.transform.root.GetComponentInChildren<PlayerController>();
            playerController.StartCoroutine( Targeting(user, playerController, onFinished) );
        }
        #endregion
    }
}