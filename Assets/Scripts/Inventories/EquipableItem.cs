using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("RPG/Inventory Item/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        #region --Fields-- (Inspector)
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] private EquipLocation _allowedEquipLocation = EquipLocation.Weapon;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public EquipLocation GetAllowedEquipLocation()
        {
            return _allowedEquipLocation;
        }
        #endregion
    }
}