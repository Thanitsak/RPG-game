using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Shops;
using System.Globalization;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Row Stuffs")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _availabilityText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _quantityText;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _removeButton;
        #endregion



        #region --Fields-- (In Class)
        private Shop _currentShop;
        private ShopItem _shopItem;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _addButton.onClick.AddListener(AddQuantity);
            _removeButton.onClick.AddListener(RemoveQuantity);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(Shop shop, ShopItem shopItem)
        {
            _currentShop = shop;
            _shopItem = shopItem;

            // Update Texts
            _iconImage.overrideSprite = shopItem.Icon;
            _nameText.text = shopItem.Name;
            _availabilityText.text = $"{shopItem.Availability}";

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _priceText.text = shopItem.Price.ToString("#,0", nfi);

            _quantityText.text = $"{shopItem.QuantityInTransaction}";
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void AddQuantity()
        {
            if (_currentShop == null || _shopItem == null) return;

            _currentShop.AddToTransaction(_shopItem.InventoryItem, 1);
        }

        private void RemoveQuantity()
        {
            if (_currentShop == null || _shopItem == null) return;

            _currentShop.AddToTransaction(_shopItem.InventoryItem, -1);
        }
        #endregion
    }
}