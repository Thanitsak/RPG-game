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
        private Shopper _shopper;

        private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        #endregion



        #region --Properties-- (With Backing Fields)
        public string ShopTitleName { get { return _shopTitleName; } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _actionScheduler = GameObject.FindWithTag("Player").GetComponent<ActionScheduler>();

            InitializeTransactionRecord();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_shopper == null) return;

                _shopper.SetActiveShop(this);
                _actionScheduler.StopCurrentAction();

                _shopper = null;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (StockItemConfig eachStock in _stockItems)
            {
                yield return new ShopItem(eachStock.inventoryItem, eachStock.initialStock, GetShopItemPrice(eachStock), _transaction[eachStock.inventoryItem]);
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

        public void ConfirmTransaction()
        {

        }

        public float GetTransactionTotal()
        {
            return 0f;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            _transaction[item] += quantity;

            if (_transaction[item] < 0)
                _transaction[item] = 0;

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

        private void InitializeTransactionRecord()
        {
            _transaction.Clear();
            foreach (StockItemConfig eachStock in _stockItems)
            {
                if (!_transaction.ContainsKey(eachStock.inventoryItem))
                    _transaction.Add(eachStock.inventoryItem, 0);
                else
                    Debug.LogError($"Can't add duplicate Stock Item under shop '{transform.parent.name}'");
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

                _shopper = playerController.GetComponentInChildren<Shopper>();
            }

            return true;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class StockItemConfig
        {
            public InventoryItem inventoryItem;
            public int initialStock;
            [Tooltip("Negative Value Mean on top on the product price, make it more expensive. Positive make it cheaper.")]
            [Range(-100f,100f)]
            public float buyingDiscountPercentage;
        }
        #endregion
    }
}