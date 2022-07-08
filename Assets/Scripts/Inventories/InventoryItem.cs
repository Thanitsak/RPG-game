using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region --Fields-- (Inspector)
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] private string _itemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] private string _displayName = null;
        [TextArea]
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField] private string _description = null;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField] private Sprite _icon = null;
        [Tooltip("The prefab that should be spawned when this item is dropped.")]
        [SerializeField] private Pickup _pickup = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField] private bool _stackable = false;
        [Tooltip("Default Price of this item when put as stock in shop system (each shop can have different discount percentage). Price Limitation is due to row UI width limit (can be changed)")]
        [Range(0, 999999999)]
        [SerializeField] private int _price = 0;
        [SerializeField] private ItemCategory _category = ItemCategory.None;
        #endregion



        #region --Fields-- (In Class)
        private static Dictionary<string, InventoryItem> _itemLookupCache;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            if (_itemLookupCache == null)
            {
                _itemLookupCache = new Dictionary<string, InventoryItem>();
                InventoryItem[] itemList = Resources.LoadAll<InventoryItem>("");
                foreach (InventoryItem item in itemList)
                {
                    if (_itemLookupCache.ContainsKey(item._itemID))
                    {
                        Debug.LogError($"Looks like there's a duplicate InventoryItem ID for objects: {_itemLookupCache[item._itemID]} and {item}");
                        continue;
                    }

                    _itemLookupCache[item._itemID] = item;
                }
                if (itemList.Length == 0) Debug.LogError($"Resources can't find any InventoryItem, so will always return null.");
            }
            if (itemID != null && !_itemLookupCache.ContainsKey(itemID)) Debug.LogError($"Resources can't find an InventoryItem: {itemID}, so will return as null.");
            if (itemID == null || !_itemLookupCache.ContainsKey(itemID)) return null;

            return _itemLookupCache[itemID];
        }

        /// <summary>
        /// Spawn the pickup gameobject into the world.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <param name="number">How many instances of the item does the pickup represent.</param>
        /// <returns>Reference to the pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var pickup = Instantiate(_pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public string GetItemID()
        {
            return _itemID;
        }

        public bool IsStackable()
        {
            return _stackable;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public string GetDescription()
        {
            return _description;
        }

        public int GetPrice()
        {
            return _price;
        }

        public ItemCategory GetCategory()
        {
            return _category;
        }
        #endregion



        #region --Methods-- (Interface)
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
        #endregion
    }
}