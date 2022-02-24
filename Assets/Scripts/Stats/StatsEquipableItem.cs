using UnityEngine;
using GameDevTV.Inventories;
using System.Collections.Generic;

namespace RPG.Stats
{
    [CreateAssetMenu(menuName = "RPG/Inventory Item/Stats Equipable Item")]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        #region --Fields-- (Inspector)
        [SerializeField]
        private Modifier[] _additiveModifiers;
        [SerializeField]
        private Modifier[] _percentageModifiers;
        #endregion



        #region --Structs-- (Custom PRIVATE)
        [System.Serializable]
        private struct Modifier
        {
            public StatType statType;
            public float value;
        }
        #endregion



        #region --Methods-- (Interface)
        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatType statType)
        {
            foreach (Modifier eachModifier in _additiveModifiers)
            {
                if (eachModifier.statType == statType)
                {
                    yield return eachModifier.value;
                }
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatType statType)
        {
            foreach (Modifier eachModifier in _percentageModifiers)
            {
                if (eachModifier.statType == statType)
                {
                    yield return eachModifier.value;
                }
            }
        }
        #endregion
    }
}