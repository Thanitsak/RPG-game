using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Inventory _inventory;

        private InventoryItem _item;
        private int _number = 1;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _inventory = player.GetComponent<Inventory>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(InventoryItem item, int number)
        {
            _item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            _number = number;
        }

        public InventoryItem GetItem()
        {
            return _item;
        }

        public int GetNumber()
        {
            return _number;
        }

        public void PickupItem()
        {
            bool foundSlot = _inventory.AddToFirstEmptySlot(_item, _number);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return _inventory.HasSpaceFor(_item);
        }
        #endregion
    }
}