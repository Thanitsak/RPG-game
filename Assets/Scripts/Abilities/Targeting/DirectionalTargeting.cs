using System;
using UnityEngine;
using RPG.Control;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Untitled Directional Targeting", menuName = "RPG/Game Item/Targeting/New Directional", order = 125)]
    public class DirectionalTargeting : TargetingStrategy
    {
        #region --Fields-- (Inspector)
        [Tooltip("Layer that the Affect will start on (Usually only on Terrain layer)")]
        [SerializeField] private LayerMask _affectStarterLayer;
        [SerializeField] private float _groundOffset = 1f;
        #endregion



        #region --Methods-- (Override)
        public override void StartTargeting(AbilityData data, Action onFinished)
        {
            PlayerController playerController = data.User.transform.root.GetComponentInChildren<PlayerController>();

            // Cast a ray from mouse to the Ground ONLY
            Ray ray = playerController.GetTouchRay();
            if (Physics.Raycast(ray, out RaycastHit hit, _affectStarterLayer))
            {
                // Using Mathematical taught in Lecture 55 in Shop & Ability Course, can make sure that this won't go inside the ground
                data.TargetedPoint = hit.point + ray.direction * _groundOffset / ray.direction.y;

                onFinished?.Invoke();
            }
        }
        #endregion
    }
}