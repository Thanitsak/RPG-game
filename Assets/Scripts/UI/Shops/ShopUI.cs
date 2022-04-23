using TMPro;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using RPG.Shops;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Text Stuffs")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _totalPriceText;
        [SerializeField] private Color _notSufficientFundsColorTotalPrice;
        [SerializeField] private TMP_Text _switchShopModeText;
        [SerializeField] private string _sellerText = "Switch to Sell";
        [SerializeField] private string _buyerText = "Switch to Buy";
        [SerializeField] private TMP_Text _confirmText;

        [Header("Panel Stuffs")]
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _confirmTransactionButton;
        [SerializeField] private Button _switchShopModeButton;

        [Header("Spawn Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private Shopper _shopper;
        private Shop _currentShop;

        private Color _totalPriceOriginalColor;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shopper>();

            // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button
            _shopper.OnActiveShopChanged += RefreshShopUI;

            _quitButton.onClick.AddListener(Quit);
            _confirmTransactionButton.onClick.AddListener(ConfirmTransaction);
            _switchShopModeButton.onClick.AddListener(SwitchShopMode);

            _totalPriceOriginalColor = _totalPriceText.color;
        }

        private void Start()
        {
            RefreshShopUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~ShopUIPanel~
        private void UpdateShopUIPanel()
        {
            gameObject.SetActive(_currentShop != null);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~ShopItemUI~
        private void BuildShopItemList()
        {
            foreach (ShopItem eachShopItem in _currentShop.GetFilteredItems())
            {
                RowUI createdPrefab = Instantiate(_rowPrefab, _spawnParent);
                createdPrefab.Setup(_currentShop, eachShopItem);
            }
        }

        private void ClearShopItemList()
        {
            foreach (Transform eachChild in _spawnParent)
                Destroy(eachChild.gameObject);
        }

        private void UpdateTotalPriceText()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _totalPriceText.text = _currentShop.GetTransactionTotal().ToString("#,0", nfi);

            switch (_currentShop.ShopMode)
            {
                case ShopMode.Seller:
                    _totalPriceText.color = _currentShop.IsShopperHasSufficientFunds() ? _totalPriceOriginalColor : _notSufficientFundsColorTotalPrice;
                    break;

                case ShopMode.Buyer:
                    _totalPriceText.color = _totalPriceOriginalColor;
                    break;
            }
        }

        private void UpdateConfirmButtonState()
        {
            _confirmTransactionButton.interactable = _currentShop.CanTransact();
        }

        private void UpdateSwitchShopModeText()
        {
            switch (_currentShop.ShopMode)
            {
                case ShopMode.Seller:
                    _switchShopModeText.text = _sellerText;
                    _confirmText.text = "Buy";
                    break;

                case ShopMode.Buyer:
                    _switchShopModeText.text = _buyerText;
                    _confirmText.text = "Sell";
                    break;
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~FilterButtons~
        private void SetupFilterButtonUI()
        {
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(_currentShop);
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshShopUI()
        {
            if (_currentShop != null)
                _currentShop.OnShopItemChanged -= RefreshShopItemUI; // Unsubscribe when there is OLD shop

            _currentShop = _shopper.GetActiveShop();

            UpdateShopUIPanel();
            SetupFilterButtonUI(); // Setup here cuz currentShop will also be null (anything fine)

            if (_currentShop == null) return;
            _currentShop.OnShopItemChanged += RefreshShopItemUI; // Subscribe on currently added shop
            _titleText.text = _currentShop.ShopTitleName;
            
            RefreshShopItemUI();
        }

        private void RefreshShopItemUI()
        {
            ClearShopItemList();

            BuildShopItemList();

            UpdateTotalPriceText();
            UpdateConfirmButtonState();

            UpdateSwitchShopModeText();
        }

        private void Quit()
        {
            _shopper.SetActiveShop(null);
        }

        private void ConfirmTransaction()
        {
            if (_currentShop == null) return;

            _currentShop.ConfirmTransaction();
        }

        private void SwitchShopMode()
        {
            if (_currentShop == null) return;

            switch (_currentShop.ShopMode)
            {
                case ShopMode.Seller:
                    _currentShop.ShopMode = ShopMode.Buyer;
                    break;

                case ShopMode.Buyer:
                    _currentShop.ShopMode = ShopMode.Seller;
                    break;
            }
        }
        #endregion
    }
}