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
        #endregion



        #region --Fields-- (In Class)
        private Shopper _shopper;
        private Shop _currentShop;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shopper>();

            _quitButton.onClick.AddListener(Quit);
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

            if (_currentShop == null) return;

            _titleText.text = _currentShop.ShopTitleName;
        }

        private void Quit()
        {
            _shopper.SetActiveShop(null);
        }
        #endregion
    }
}