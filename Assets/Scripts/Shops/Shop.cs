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
        /*
        --NOTE--
        Question Idea : Selling Items that are NOT in _stockItems
        Solution : simply create new stockItem for Selling instead and we can use that for selling

        Question Idea : Selling Items that are based on Player's Inventory
        Solution : create new StockItemConfig List from player inventory and display that list in selling mode
         */

        #region --Fields-- (Inspector)
        [SerializeField] private ShopMode _shopMode = ShopMode.Seller;
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

        private ItemCategory _currentFilter = ItemCategory.None;
        #endregion



        #region --Fields-- (Constant)
        private const Int16 MaxQuantity = 999;
        #endregion



        #region --Properties-- (With Backing Fields)
        public string ShopTitleName { get { return _shopTitleName; } }

        public ShopMode ShopMode
        {
            get
            {
                return _shopMode;
            }
            set
            {
                _shopMode = value;

                _transaction.Clear();
                OnShopItemChanged?.Invoke();
            }
        }

        public ItemCategory CurrentFilter
        {
            get
            {
                return _currentFilter;
            }
            set
            {
                _currentFilter = value;
                print($"{_currentFilter} / Shop : {gameObject.transform.parent.name} / Root : {gameObject.transform.root.name}");
                OnShopItemChanged?.Invoke();
            }
        }
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

                _transaction.Clear();
                CurrentShopper.SetActiveShop(this);
                _actionScheduler.StopCurrentAction();

                // _currentShopper will be set to null in Shopper.SetActiveShop(null) by quit button in ShopUI
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Shop Items Filtering~
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

                yield return new ShopItem(eachStock.inventoryItem, GetAvailableQuantity(eachStock.inventoryItem), GetShopItemPrice(eachStock), quantityInTransaction);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Shop Transaction~
        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;

            if (ShopMode == ShopMode.Seller)
            {
                if (!IsShopperHasSufficientFunds()) return false;
                if (!IsShopperHasInventorySpace()) return false;
            }

            return true;
        }

        public bool IsTransactionEmpty() => _transaction.Count == 0;

        public bool IsShopperHasSufficientFunds()
        {
            Coin shopperCoin = CurrentShopper.transform.root.GetComponentInChildren<Coin>();
            if (shopperCoin == null) return false;

            return shopperCoin.CoinPoints >= GetTransactionTotal();
        }

        public bool IsShopperHasInventorySpace()
        {
            Inventory shopperInventory = CurrentShopper.transform.root.GetComponentInChildren<Inventory>();
            if (shopperInventory == null) return false;

            // Build InventoryItem List from ShopItem by using its quantity
            List<InventoryItem> flatInventoryItems = new List<InventoryItem>(); // flat mean convert 2D to 1D (2D in this case is ShopItems each one has different quantity)
            foreach (ShopItem shopItem in GetAllItems())
            {
                for (int i = 0; i < shopItem.QuantityInTransaction; i++)
                    flatInventoryItems.Add(shopItem.InventoryItem);
            }

            return shopperInventory.HasSpaceFor(flatInventoryItems);
        }

        /// <summary>
        /// Get Called by Buy/Sell button. Transfer TO/FROM inventory / Removal from Transaction / Debting or Crediting player moneys
        /// </summary>
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = CurrentShopper.transform.root.GetComponentInChildren<Inventory>();
            Coin shopperCoin = CurrentShopper.transform.root.GetComponentInChildren<Coin>();
            if (shopperInventory == null || shopperCoin == null) return;

            foreach (ShopItem shopItem in GetAllItems())
            {
                switch (ShopMode)
                {
                    case ShopMode.Seller:
                        BuyItemRepeatedly(shopperInventory, shopperCoin, shopItem.InventoryItem, shopItem.QuantityInTransaction, shopItem.Price);
                        break;
                    case ShopMode.Buyer:
                        SellItemRepeatedly(shopperInventory, shopperCoin, shopItem.InventoryItem, shopItem.QuantityInTransaction, shopItem.Price);
                        break;
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

        public void UpdateTransaction(InventoryItem item, int quantity)
        {
            // Add to Transaction if it doesn't yet exist
            if (!_transaction.ContainsKey(item))
                _transaction.Add(item, 0);

            // Add/Remove quantity (if -num then remove)
            _transaction[item] += quantity;

            // Clamping & Remove if its quantity is 0
            _transaction[item] = Mathf.Clamp(_transaction[item], 0, GetAvailableQuantity(item));
            if (_transaction[item] == 0)
                _transaction.Remove(item);

            OnShopItemChanged?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private int GetShopItemPrice(StockItemConfig stockItem)
        {
            float discountPercentage = 0f;
            switch (ShopMode)
            {
                case ShopMode.Seller:
                    discountPercentage = stockItem.buyingDiscountPercentage;
                    break;
                case ShopMode.Buyer:
                    discountPercentage = stockItem.sellingDiscountPercentage;
                    break;
            }

            int defaultPrice = stockItem.inventoryItem.GetPrice();
            float discountAmount = (defaultPrice / 100f) * (-discountPercentage); // negate so that positive percentage mean deduct out of defaultPrice & negative percentage mean add on to defaultPrice
            
            return (int)Math.Round(defaultPrice + discountAmount, MidpointRounding.AwayFromZero); //2.5 will be 3
        }

        private int GetAvailableQuantity(InventoryItem inventoryItem)
        {
            switch (ShopMode)
            {
                case ShopMode.Seller:
                    return _availableQuantity[inventoryItem];

                case ShopMode.Buyer:
                    Inventory shopperInventory = CurrentShopper.transform.root.GetComponentInChildren<Inventory>();
                    if (shopperInventory == null) return -1;

                    return shopperInventory.CountItemInAllSlots(inventoryItem);
            }

            return -1;
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

        private void BuyItemRepeatedly(Inventory shopperInventory, Coin shopperCoin, InventoryItem inventoryItem, int quantity, int price)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (shopperCoin.CoinPoints < price) return; // exit early when don't have enough money

                bool success = shopperInventory.AddToFirstEmptySlot(inventoryItem, 1);
                if (!success) return; // exit early when slot full

                UpdateTransaction(inventoryItem, -1);
                shopperCoin.UpdateCoinPoints(-price);
                _availableQuantity[inventoryItem] -= 1;
            }
        }

        private void SellItemRepeatedly(Inventory shopperInventory, Coin shopperCoin, InventoryItem inventoryItem, int quantity, int price)
        {
            for (int i = 0; i < quantity; i++)
            {
                bool success = shopperInventory.RemoveFirstMatchItemFromSlot(inventoryItem, 1);
                if (!success) return; // exit early when slot full

                UpdateTransaction(inventoryItem, -1);
                shopperCoin.UpdateCoinPoints(price);
                _availableQuantity[inventoryItem] += 1;
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
            [Tooltip("Player Get Discount when BUY from Shop, Positive -> LOWER default price / Negative -> HIGHER default price")]
            [Range(-100f,100f)]
            public float buyingDiscountPercentage;
            [Tooltip("Player Get Discount when SELL to Shop, Positive -> LOWER default price / Negative -> HIGHER default price")]
            [Range(-100f, 100f)]
            public float sellingDiscountPercentage;
        }
        #endregion
    }
}