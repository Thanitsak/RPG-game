using UnityEngine;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] InventorySlotUI InventoryItemPrefab = null;
        #endregion



        #region --Fields-- (In Class)
        Inventory playerInventory;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.inventoryUpdated += Redraw;
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

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(InventoryItemPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }
        #endregion
    }
}