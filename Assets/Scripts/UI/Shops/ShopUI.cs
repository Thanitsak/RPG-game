using TMPro;
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

        [Header("Panel Stuffs")]
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _confirmTransactionButton;

        [Header("Spawn Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private Shopper _shopper;
        private Shop _currentShop;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shopper>();

            // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button
            _shopper.OnActiveShopChanged += RefreshShopUI;

            _quitButton.onClick.AddListener(Quit);
            _confirmTransactionButton.onClick.AddListener(ConfirmTransaction);
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
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshShopUI()
        {
            if (_currentShop != null)
                _currentShop.OnShopItemChanged -= RefreshShopItemUI; // Unsubscribe when there is OLD shop

            _currentShop = _shopper.GetActiveShop();

            UpdateShopUIPanel();

            if (_currentShop == null) return;
            _currentShop.OnShopItemChanged += RefreshShopItemUI; // Subscribe on currently added shop
            _titleText.text = _currentShop.ShopTitleName;
            
            RefreshShopItemUI();
        }

        private void RefreshShopItemUI()
        {
            ClearShopItemList();

            BuildShopItemList();
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
        #endregion
    }
}