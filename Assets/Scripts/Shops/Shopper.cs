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
        public void SetActiveShop(Shop shopInput)
        {
            // Set CurrentShopper back to Null in activeShop when passed in as null
            if (shopInput == null && _activeShop != null)
                _activeShop.CurrentShopper = null;
            
            _activeShop = shopInput;
            
            OnActiveShopChanged?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return _activeShop;
        }
        #endregion
    }
}