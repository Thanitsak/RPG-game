using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Inventories;
using RPG.Control;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _shopTitleName;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnShopUpdated;
        #endregion



        #region --Fields-- (In Class)
        private Shopper _shopper;
        #endregion



        #region --Properties-- (With Backing Fields)
        public string ShopTitleName { get { return _shopTitleName; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            return null;
        }

        public void SelectFilter(ItemCategory itemCategory)
        {

        }

        public ItemCategory GetCurrentFilter()
        {
            return ItemCategory.None;
        }

        public void SelectShopMode(bool isBuying)
        {

        }

        public bool IsBuyingMode()
        {
            return true;
        }

        public bool CanTransact()
        {
            return true;
        }

        public void ConfirmTransaction()
        {

        }

        public float GetTransactionTotal()
        {
            return 0f;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {

        }
        #endregion



        #region --Methods-- (Interface)
        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Shop;
        }

        bool IRaycastable.HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _shopper = playerController.GetComponentInChildren<Shopper>();

                _shopper.SetActiveShop(this);
            }

            return true;
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        public class ShopItem
        {
            public InventoryItem item;
            public int availability;
            public float price;
            public int quantityInTransaction;
        }
        #endregion
    }
}