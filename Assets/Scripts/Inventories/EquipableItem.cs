using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(fileName = "Untitled (Equipable)", menuName = "RPG/Game Item/New (Equipable)", order = 100)]
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