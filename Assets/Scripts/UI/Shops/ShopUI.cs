using UnityEngine;
using RPG.Shops;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Shopper _shopper;
        private Shop _currentShop;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shopper>();
        }

        private void OnEnable()
        {
            _shopper.OnActiveShopChanged += UpdateShopUI;
        }

        private void Start()
        {
            UpdateShopUI();
        }

        // Can't Have OnDiable() to unsubscribe Since this one will be closed by default and with button
        #endregion


        #region --Methods-- (Subscriber)
        private void UpdateShopUI()
        {
            _currentShop = _shopper.GetActiveShop();
            gameObject.SetActive(_currentShop != null);
        }
        #endregion
    }
}