using UnityEngine;
using RPG.Inventories;
using RPG.Core;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private InventorySlotUI _inventoryItemPrefab = null;
        #endregion



        #region --Fields-- (In Class)
        private Inventory _playerInventory;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerInventory = Inventory.GetPlayerInventory();

            UIRefresher.OnInventoryBagRefreshed += Redraw; // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button
        }

        private void Start()
        {
            Redraw();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            // TODO Improve Performance with Object Pooling NOT KEEPING INSTANTIATE LIKE THIS!!!
            for (int i = 0; i < _playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(_inventoryItemPrefab, transform);
                itemUI.Setup(_playerInventory, i);
            }
        }
        #endregion
    }
}