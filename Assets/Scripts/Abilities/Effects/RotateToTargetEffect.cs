using System.Collections;
using System;
using UnityEngine;
using RPG.Utils.Core;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Untitled Rotate To Target Effect", menuName = "RPG/Game Item/Effects/New Rotate To Target Effect", order = 131)]
    public class RotateToTargetEffect : EffectStrategy
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _rotationSpeed = 10f;
        [Tooltip("0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f")]
        [SerializeField] private float _precisionRate = 0.0000001f;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator KeepRotating(AbilityData data, Action<string> onFinished)
        {
            while (Utilities.SmoothRotateTo(data.User.transform, data.TargetedPoint, _rotationSpeed, _precisionRate) && data.IsAbilityCancelled == false) // when ability is cancelled via move, attack or die this will stop rotating
            {
                yield return null;
            }

            onFinished?.Invoke(name);
        }
        #endregion



        #region --Methods-- (Override)
        public override void StartEffect(AbilityData data, Action<string> onFinished)
        {
            data.StartCoroutine( KeepRotating(data, onFinished) );
        }
        #endregion
    }
}