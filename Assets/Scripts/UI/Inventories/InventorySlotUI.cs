using UnityEngine;
using RPG.Inventories;
using RPG.Utils.UI.Dragging;

namespace RPG.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region --Fields-- (Inspector)
        [SerializeField] private InventoryItemIcon _icon = null;
        #endregion



        #region --Fields-- (In Class)
        private int _index;
        private InventoryItem _item;
        private Inventory _inventory;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(Inventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
            _icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (_inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _inventory.AddItemToSlot(_index, item, number);
        }

        public InventoryItem GetItem()
        {
            return _inventory.GetItemInSlot(_index);
        }

        public int GetNumber()
        {
            return _inventory.GetNumberInSlot(_index);
        }

        public void RemoveItems(int number)
        {
            _inventory.RemoveFromSlot(_index, number);
        }
        #endregion
    }
}