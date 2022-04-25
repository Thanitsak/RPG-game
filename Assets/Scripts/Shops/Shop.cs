using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Inventories;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using RPG.Economy;
using RPG.Stats;

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
        [SerializeField] private ItemCategory _itemFilter = ItemCategory.None;
        [SerializeField] private string _shopTitleName;
        [SerializeField] private StockItemConfig[] _stockItems;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnShopItemChanged;
        #endregion



        #region --Fields-- (In Class)
        private ActionScheduler _actionScheduler;

        private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> _quantitySold = new Dictionary<InventoryItem, int>();
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

        public ItemCategory ItemFilter
        {
            get
            {
                return _itemFilter;
            }
            set
            {
                _itemFilter = value;
                
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
            foreach (ShopItem shopItem in GetAllItems())
            {
                if (ItemFilter == ItemCategory.None || ItemFilter == shopItem.InventoryItem.GetCategory())
                    yield return shopItem;
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, int> prices = GetPrices();
            Dictionary<InventoryItem, int> availibilities = GetAvailibilities();

            foreach (InventoryItem item in availibilities.Keys)
            {
                //if (availibilities[item] <= 0) continue; **UNCOMMENT this to not showing empty item**

                // IF item does NOT exist this won't throw error and quantity = 0, IF exist quantity = item's value
                _transaction.TryGetValue(item, out int quantityInTransaction);

                //yield return new ShopItem(item, GetAvailableQuantity(eachStock.inventoryItem), GetShopItemPrice(eachStock), quantityInTransaction);
                yield return new ShopItem(item, availibilities[item], prices[item], quantityInTransaction);
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

            Dictionary<InventoryItem, int> prices = GetPrices();
            foreach (InventoryItem item in _transaction.Keys)
            {
                totalPrice += _transaction[item] * prices[item];
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
            Dictionary<InventoryItem, int> availiability = GetAvailibilities();
            _transaction[item] = Mathf.Clamp(_transaction[item], 0, availiability[item]);
            if (_transaction[item] == 0)
                _transaction.Remove(item);

            OnShopItemChanged?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private Dictionary<InventoryItem, int> GetPrices()
        {
            Dictionary<InventoryItem, int> prices = new Dictionary<InventoryItem, int>();
            Dictionary<InventoryItem, int> highestUnlockLevel = new Dictionary<InventoryItem, int>();

            foreach (StockItemConfig stockItem in GetAvailiableConfigs())
            {
                InventoryItem item = stockItem.inventoryItem;

                if (!prices.ContainsKey(item))
                    prices.Add(item, 0);

                // ONLY USE discount percentage from stock with HIGHEST levelToUnlock
                if (!highestUnlockLevel.ContainsKey(item) || stockItem.levelToUnlock > highestUnlockLevel[item])
                {
                    highestUnlockLevel[item] = stockItem.levelToUnlock;

                    float discountPercentage = 0f;
                    if (ShopMode == ShopMode.Seller)
                        discountPercentage = stockItem.buyingDiscountPercentage;
                    else if (ShopMode == ShopMode.Buyer)
                        discountPercentage = stockItem.sellingDiscountPercentage;

                    int defaultPrice = stockItem.inventoryItem.GetPrice();
                    float discountAmount = (defaultPrice / 100f) * (-discountPercentage); // negate so that positive percentage mean deduct out of defaultPrice & negative percentage mean add on to defaultPrice

                    prices[item] = (int)Math.Round(defaultPrice + discountAmount, MidpointRounding.AwayFromZero); //2.5 will be 3
                }   
            }
            return prices;
        }

        private Dictionary<InventoryItem, int> GetAvailibilities()
        {
            Dictionary<InventoryItem, int> availibilities = new Dictionary<InventoryItem, int>();

            Inventory shopperInventory = CurrentShopper.transform.root.GetComponentInChildren<Inventory>();
            if (shopperInventory == null) return availibilities;

            foreach (StockItemConfig stockItem in GetAvailiableConfigs())
            {
                InventoryItem item = stockItem.inventoryItem;

                if (ShopMode == ShopMode.Seller)
                {
                    if (!availibilities.ContainsKey(item))
                    {
                        _quantitySold.TryGetValue(item, out int sold);
                        availibilities[item] = -sold;
                    }
                    availibilities[item] += stockItem.initialStock;
                }
                else if (ShopMode == ShopMode.Buyer)
                {
                    availibilities[item] = shopperInventory.CountItemInAllSlots(item);
                }
            }
            return availibilities;
        }

        private IEnumerable<StockItemConfig> GetAvailiableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (StockItemConfig stockItem in _stockItems)
            {
                if (shopperLevel >= stockItem.levelToUnlock)
                    yield return stockItem;
            }
        }

        private int GetShopperLevel()
        {
            BaseStats baseStats = CurrentShopper.transform.root.GetComponentInChildren<BaseStats>();
            if (baseStats == null) return 0;

            return baseStats.GetLevel();
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

                if (!_quantitySold.ContainsKey(inventoryItem))
                    _quantitySold[inventoryItem] = 0;
                _quantitySold[inventoryItem] += 1;
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

                if (!_quantitySold.ContainsKey(inventoryItem))
                    _quantitySold[inventoryItem] = 0;
                _quantitySold[inventoryItem] -= 1;
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
            [Tooltip("Level for this item to available for sales in shop.")]
            public int levelToUnlock = 0;
        }
        #endregion
    }
}