using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Dictionary<Ability, float> _cooldownTimers = new Dictionary<Ability, float>();
        #endregion



        #region --Methods-- (Built In)
        private void Update()
        {
            var keys = new List<Ability>(_cooldownTimers.Keys);
            foreach (Ability key in keys)
            {
                _cooldownTimers[key] -= Time.deltaTime;

                if (_cooldownTimers[key] <= 0f)
                {
                    _cooldownTimers.Remove(key);
                }
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartTimer(Ability ability, float cooldownTime)
        {
            _cooldownTimers.Add(ability, cooldownTime);
        }

        public float GetTimeRemaining(Ability ability)
        {
            if (!_cooldownTimers.ContainsKey(ability)) return 0f;

            return _cooldownTimers[ability];
        }
        #endregion
    }
}