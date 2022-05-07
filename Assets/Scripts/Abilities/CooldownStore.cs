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
        public void StartTimer(InventoryItem item, float cooldownTime)
        {
            _cooldownTimers.Add(item, cooldownTime);
            _cooldownInitials.Add(item, cooldownTime);
        }

        public float GetTimeRemaining(InventoryItem item)
        {
            if (item == null || !_cooldownTimers.ContainsKey(item)) return 0f;

            return _cooldownTimers[item];
        }

        public float GetFractionRemaining(InventoryItem item)
        {
            if (item == null || !_cooldownInitials.ContainsKey(item)) return 0f;

            return Mathf.InverseLerp(0f, _cooldownInitials[item], _cooldownTimers[item]);
        }
        #endregion
    }
}