using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Inventories;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using RPG.Economy;

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
        private Dictionary<InventoryItem, int> _availableQuantity = new Dictionary<InventoryItem, int>();
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

            CheckForDuplicateStock();

            InitializeAvailableQuantity();
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
            return GetAllItems();
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            foreach (StockItemConfig eachStock in _stockItems)
            {
                // IF item does NOT exist this won't throw error and quantity = 0, IF exist quantity = item's value
                _transaction.TryGetValue(eachStock.inventoryItem, out int quantityInTransaction);

                yield return new ShopItem(eachStock.inventoryItem, _availableQuantity[eachStock.inventoryItem], GetShopItemPrice(eachStock), quantityInTransaction);
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
            Coin shopperCoin = CurrentShopper.transform.root.GetComponentInChildren<Coin>();
            if (shopperInventory == null || shopperCoin == null) return;

            // Transfer TO/FROM inventory
            // Removal from Transaction
            // Debting or Crediting player moneys
            foreach (ShopItem eachShopItem in GetAllItems())
            {
                for (int i = 0; i < eachShopItem.QuantityInTransaction; i++)
                {
                    if (shopperCoin.CoinPoints < eachShopItem.Price) break; // exit early when don't have enough money

                    bool success = shopperInventory.AddToFirstEmptySlot(eachShopItem.InventoryItem, 1);
                    if (!success) break; // exit early when slot full

                    AddToTransaction(eachShopItem.InventoryItem, -1);
                    shopperCoin.AddCoinPoints(-eachShopItem.Price);
                    _availableQuantity[eachShopItem.InventoryItem] -= 1;
                }
            }

            OnShopItemChanged?.Invoke();
        }

        public int GetTransactionTotal()
        {
            int totalPrice = 0;

            foreach (StockItemConfig eachStock in _stockItems)
            {
                if (!_transaction.ContainsKey(eachStock.inventoryItem)) continue;

                int quantityInTransaction = _transaction[eachStock.inventoryItem];
                totalPrice += quantityInTransaction * GetShopItemPrice(eachStock);
            }

            return totalPrice;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            // Add to Transaction if it doesn't yet exist
            if (!_transaction.ContainsKey(item))
                _transaction.Add(item, 0);

            // Add/Remove quantity (if -num then remove)
            _transaction[item] += quantity;

            // Clamping & Remove if its quantity is 0
            _transaction[item] = Mathf.Clamp(_transaction[item], 0, _availableQuantity[item]);
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

        private void CheckForDuplicateStock()
        {
            Dictionary<InventoryItem, int> tempStockRecord = new Dictionary<InventoryItem, int>();
            foreach (StockItemConfig eachStock in _stockItems)
            {
                if (!tempStockRecord.ContainsKey(eachStock.inventoryItem))
                    tempStockRecord.Add(eachStock.inventoryItem, 0);
                else
                    Debug.LogError($"At '{transform.parent.name}' - Can't Add Duplicate Stock Item, '{eachStock.inventoryItem.GetDisplayName()}' is already Added");
            }
            tempStockRecord.Clear();
        }

        private void InitializeAvailableQuantity()
        {
            foreach (StockItemConfig eachStock in _stockItems)
            {
                if (!_availableQuantity.ContainsKey(eachStock.inventoryItem))
                    _availableQuantity.Add(eachStock.inventoryItem, eachStock.initialStock);
            }
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