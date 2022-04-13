using UnityEngine;
using RPG.Inventories;

namespace RPG.Shops
{
    public class ShopItem
    {
        #region --Fields-- (In Class)
        private InventoryItem _inventoryItem;
        private int _availability;
        private int _price;
        private int _quantityInTransaction;
        #endregion



        #region --Properties-- (With Backing Fields)
        public InventoryItem InventoryItem { get { return _inventoryItem; } }
        public string Name { get { return _inventoryItem.GetDisplayName(); } }
        public Sprite Icon { get { return _inventoryItem.GetIcon(); } }
        public int Availability { get { return _availability; } }
        public int Price { get { return _price; } }
        public int QuantityInTransaction { get { return _quantityInTransaction; } }
        #endregion



        #region --Constructors-- (PUBLIC)
        public ShopItem(InventoryItem inventoryItem, int availability, int price, int quantityInTransaction)
        {
            _inventoryItem = inventoryItem;
            _availability = availability;
            _price = price;
            _quantityInTransaction = quantityInTransaction;
        }
        #endregion
    }
}