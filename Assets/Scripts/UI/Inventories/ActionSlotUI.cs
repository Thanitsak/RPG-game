using RPG.Utils.UI.Dragging;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.UI;
using RPG.Abilities;
using RPG.Core;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region --Fields-- (Inspector)
        [SerializeField] private InventoryItemIcon _icon = null;
        [SerializeField] private int _index = 0;
        [SerializeField] private Image _cooldownOverlayImage;
        #endregion



        #region --Fields-- (In Class)
        private ActionStore _store;

        private CooldownStore _cooldownStore;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _store = player.GetComponentInChildren<ActionStore>();
            _cooldownStore = player.GetComponentInChildren<CooldownStore>();

            UIRefresher.OnInventoryActionRefreshed += UpdateIcon; // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button

            GetComponent<Button>().onClick.AddListener(UseAbilitiesWithButton);
        }

        private void Update()
        {
            _cooldownOverlayImage.fillAmount = _cooldownStore.GetFractionRemaining(GetItem());
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void AddItems(InventoryItem item, int number)
        {
            _store.AddAction(item, _index, number);
        }

        public InventoryItem GetItem()
        {
            return _store.GetAction(_index);
        }

        public int GetNumber()
        {
            return _store.GetNumber(_index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return _store.MaxAcceptable(item, _index);
        }

        public void RemoveItems(int number)
        {
            _store.RemoveItems(_index, number);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateIcon()
        {
            _icon.SetItem(GetItem(), GetNumber());
        }

        private void UseAbilitiesWithButton()
        {
            _store.Use(_index, _store.gameObject);
        }
        #endregion
    }
}