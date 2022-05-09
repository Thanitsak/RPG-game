using System.Collections;
using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Untitled Delay Composite Effect", menuName = "RPG/Game Item/Effects/New Delay Composite Effect", order = 132)]
    public class DelayCompositeEffect : EffectStrategy
    {
        #region --Fields-- (Inspector)
        [Tooltip("Time in seconds to delay the effects")]
        [SerializeField] private float _delay = 0f;
        [Tooltip("Effects to be delayed (can even include other delay composite effect)")]
        [SerializeField] private EffectStrategy[] _effectsToDelay;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator DelayedEffect(AbilityData data, Action<string> onFinished)
        {
            yield return new WaitForSeconds(_delay);

            foreach (EffectStrategy eachEffect in _effectsToDelay)
            {
                eachEffect.StartEffect(data, onFinished);
            }

            onFinished?.Invoke(name);
        }
        #endregion



        #region --Methods-- (Override)
        public override void StartEffect(AbilityData data, Action<string> onFinished)
        {
            data.StartCoroutine(DelayedEffect(data, onFinished));
        }
        #endregion
    }
}