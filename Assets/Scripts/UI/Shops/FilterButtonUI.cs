using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;
using RPG.Shops;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private ItemCategory _itemCategory;
        #endregion



        #region --Fields-- (In Class)
        private Button _button;
        private Shop _currentShop;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetFilter);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetShop(Shop shop)
        {
            _currentShop = shop;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void SetFilter()
        {
            if (_currentShop == null) return;

            _currentShop.CurrentFilter = _itemCategory;
        }
        #endregion
    }
}