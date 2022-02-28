using System.Collections.Generic;
using RPG.Stats;

namespace RPG.Inventories.Enhancement
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        #region --Methods-- (Interface)
        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatType statType)
        {
            foreach (EquipLocation eachSlotLocation in GetAllPopulatedSlots())
            {
                IModifierProvider equipableItem = GetItemInSlot(eachSlotLocation) as IModifierProvider; // Only Get EquipableItem that has Stats by checking whether it implement IModifierProvider or not
                if (equipableItem == null) continue;

                foreach (float eachAdditiveModifier in equipableItem.GetAdditiveModifiers(statType))
                {
                    yield return eachAdditiveModifier;
                }
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatType statType)
        {
            foreach (EquipLocation eachSlotLocation in GetAllPopulatedSlots())
            {
                IModifierProvider equipableItem = GetItemInSlot(eachSlotLocation) as IModifierProvider; // Only Get EquipableItem that has Stats by checking whether it implement IModifierProvider or not
                if (equipableItem == null) continue;

                foreach (float eachPercentageModifier in equipableItem.GetPercentageModifiers(statType))
                {
                    yield return eachPercentageModifier;
                }
            }
        }
        #endregion
    }
}