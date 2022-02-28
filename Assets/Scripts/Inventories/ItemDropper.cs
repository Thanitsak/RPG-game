using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        #region --Fields-- (In Class)
        private List<Pickup> _droppedItems = new List<Pickup>();
        private List<DropRecord> _otherSceneDroppedItems = new List<DropRecord>();
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }
        #endregion



        #region --Methods-- (Custom PROTECTED)
        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            _droppedItems.Add(pickup);
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in _droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            _droppedItems = newList;
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new List<DropRecord>();
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            foreach (Pickup eachPickup in _droppedItems)
            {
                DropRecord item = new DropRecord();
                item.itemID = eachPickup.GetItem().GetItemID();
                item.position = new SerializableVector3(eachPickup.transform.position);
                item.number = eachPickup.GetNumber();
                item.sceneBuildIndex = sceneBuildIndex;

                droppedItemsList.Add(item);
            }
            droppedItemsList.AddRange(_otherSceneDroppedItems);

            return droppedItemsList;
        }

        void ISaveable.RestoreState(object state)
        {
            List<DropRecord> droppedItemsList = (List<DropRecord>)state;
            int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            _otherSceneDroppedItems.Clear();
            foreach (DropRecord eachItem in droppedItemsList)
            {
                if (eachItem.sceneBuildIndex != currentSceneBuildIndex)
                {
                    _otherSceneDroppedItems.Add(eachItem);
                    continue;
                }

                InventoryItem pickupItem = InventoryItem.GetFromID(eachItem.itemID);
                Vector3 position = eachItem.position.ToVector();
                int number = eachItem.number;

                SpawnPickup(pickupItem, position, number);
            }
        }
        #endregion



        #region --Structs-- (Custom PRIVATE)
        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneBuildIndex;
        }
        #endregion
    }
}