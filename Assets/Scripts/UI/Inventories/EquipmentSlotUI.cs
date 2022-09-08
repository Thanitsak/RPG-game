using UnityEngine;
using RPG.Utils.UI.Dragging;
using RPG.Inventories;
using RPG.Core;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region --Fields-- (Inspector)
        [SerializeField] private InventoryItemIcon _icon = null;
        [SerializeField] private EquipLocation _equipLocation = EquipLocation.Weapon;
        #endregion



        #region --Fields-- (In Class)
        private GameObject _player;
        private Equipment _playerEquipment;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerEquipment = _player.GetComponentInChildren<Equipment>();
            
            UIRefresher.OnInventoryEquipmentRefreshed += RedrawUI; // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button
        }

        private void Start()
        {
            RedrawUI();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (!equipableItem.CanEquip(_equipLocation, _player)) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _playerEquipment.AddItem(_equipLocation, (EquipableItem)item);
        }

        public InventoryItem GetItem()
        {
            return _playerEquipment.GetItemInSlot(_equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            _playerEquipment.RemoveItem(_equipLocation);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RedrawUI()
        {
            _icon.SetItem(_playerEquipment.GetItemInSlot(_equipLocation));
        }
        #endregion
    }
}