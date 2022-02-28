using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private InventoryItem _item = null;
        [SerializeField] private int _number = 1;
        #endregion



        #region --Fields-- (In Class)
        private bool _isCollectedSave = false;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup()
        {
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected()
        {
            return GetPickup() == null;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void SpawnPickup()
        {
            var spawnedPickup = _item.SpawnPickup(transform.position, _number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return isCollected() || _isCollectedSave; // Need '_isCollectedSave' bcuz 'isCollected()' will return 'false' when load scene and will override save file as if we havn't pickup!
        }

        void ISaveable.RestoreState(object state)
        {
            _isCollectedSave = (bool)state;

            if (_isCollectedSave)
            {
                DestroyPickup();
            }

            if (!_isCollectedSave)
            {
                SpawnPickup();
            }
        }
        #endregion
    }
}