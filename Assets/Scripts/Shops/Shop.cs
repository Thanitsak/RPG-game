using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Inventories;
using RPG.Control;
using RPG.Core;
using RPG.Movement;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _shopTitleName;
        [SerializeField] private StockItemConfig[] _stockItems;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnShopItemChanged;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        #endregion



        #region --Fields-- (Constant)
        private const Int16 MaxQuantity = 999;
        #endregion



        #region --Properties-- (With Backing Fields)
        public string ShopTitleName { get { return _shopTitleName; } }
        #endregion



        #region --Properties-- (Auto)
        public Shopper CurrentShopper { get; set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GameObject.FindWithTag("Player").GetComponent<ActionScheduler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (CurrentShopper == null) return;

                CurrentShopper.SetActiveShop(this);
                _actionScheduler.StopCurrentAction();

                // _currentShopper will be set to null in Shopper.SetActiveShop(null) by quit button in ShopUI
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (StockItemConfig eachStock in _stockItems)
            {
                // IF item does NOT exist this won't throw error and quantity = 0, IF exist quantity = item's value
                _transaction.TryGetValue(eachStock.inventoryItem, out int quantityInTransaction);

                yield return new ShopItem(eachStock.inventoryItem, eachStock.initialStock, GetShopItemPrice(eachStock), quantityInTransaction);
            }
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

        /// <summary>
        /// Get Called by Buy/Sell button
        /// </summary>
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = CurrentShopper.transform.root.GetComponentInChildren<Inventory>();
            if (shopperInventory == null) return;

            // Transfer TO/FROM inventory
            var transactionSnapshot = new Dictionary<InventoryItem, int>(_transaction);
            foreach (var each in transactionSnapshot)
            {
                // For Each of Item, Gradually Add one Reward to empty slot (Stackable or Non-Stackable can both be done like this)
                for (int i = 0; i < each.Value; i++)
                {
                    bool success = shopperInventory.AddToFirstEmptySlot(each.Key, 1);
                    if (success)
                        AddToTransaction(each.Key, -1);
                    else
                        break;
                }
            }

            // TODO : Removal from Transaction
            // TODO : Debting or Crediting player moneys
        }

        public float GetTransactionTotal()
        {
            return 0f;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            // Add to Transaction if it doesn't yet exist
            if (!_transaction.ContainsKey(item))
                _transaction.Add(item, 0);

            // Add/Remove quantity (if -num then remove)
            _transaction[item] += quantity;

            // Clamping & Remove if its quantity is 0
            _transaction[item] = Mathf.Clamp(_transaction[item], 0, MaxQuantity);
            if (_transaction[item] == 0)
                _transaction.Remove(item);

            OnShopItemChanged?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private int GetShopItemPrice(StockItemConfig stockItem)
        {
            int defaultPrice = stockItem.inventoryItem.GetPrice();
            float discountAmount = (defaultPrice / 100f) * (-stockItem.buyingDiscountPercentage); // negate so that positive percentage mean deduct out of defaultPrice & negative percentage mean add on to defaultPrice
            
            return (int)Math.Round(defaultPrice + discountAmount, MidpointRounding.AwayFromZero); //2.5 will be 3
        }

        // TODO : method for checking duplicate stock
        //private void InitializeTransactionRecord()
        //{
        //    _transaction.Clear();
        //    foreach (StockItemConfig eachStock in _stockItems)
        //    {
        //        if (!_transaction.ContainsKey(eachStock.inventoryItem))
        //            _transaction.Add(eachStock.inventoryItem, 0);
        //        else
        //            Debug.LogError($"Can't add duplicate Stock Item under shop '{transform.parent.name}'");
        //    }
        //}
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
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);

                CurrentShopper = playerController.GetComponentInChildren<Shopper>();
            }

            return true;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class StockItemConfig
        {
            public InventoryItem inventoryItem;
            [Range(0, MaxQuantity)]
            public int initialStock;
            [Tooltip("Negative Value Mean on top on the product price, make it more expensive. Positive make it cheaper.")]
            [Range(-100f,100f)]
            public float buyingDiscountPercentage;
        }
        #endregion
    }
}