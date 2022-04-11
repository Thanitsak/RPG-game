using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public event Action OnActiveShopChanged;
        #endregion



        #region --Fields-- (In Class)
        private Shop _activeShop = null;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetActiveShop(Shop shop)
        {
            _activeShop = shop;
            OnActiveShopChanged?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return _activeShop;
        }
        #endregion
    }
}