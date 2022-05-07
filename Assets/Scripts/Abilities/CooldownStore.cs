using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Dictionary<InventoryItem, float> _cooldownTimers = new Dictionary<InventoryItem, float>();
        private Dictionary<InventoryItem, float> _cooldownInitials = new Dictionary<InventoryItem, float>();
        #endregion



        #region --Methods-- (Built In)
        private void Update()
        {
            var keys = new List<InventoryItem>(_cooldownTimers.Keys);
            foreach (Ability key in keys)
            {
                _cooldownTimers[key] -= Time.deltaTime;

                if (_cooldownTimers[key] <= 0f)
                {
                    _cooldownTimers.Remove(key);
                    _cooldownInitials.Remove(key);
                }
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartTimer(InventoryItem ability, float cooldownTime)
        {
            _cooldownTimers.Add(ability, cooldownTime);
            _cooldownInitials.Add(ability, cooldownTime);
        }

        public float GetTimeRemaining(InventoryItem ability)
        {
            if (ability == null || !_cooldownTimers.ContainsKey(ability)) return 0f;

            return _cooldownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if (ability == null || !_cooldownInitials.ContainsKey(ability)) return 0f;

            return Mathf.InverseLerp(0f, _cooldownInitials[ability], _cooldownTimers[ability]);
        }
        #endregion
    }
}