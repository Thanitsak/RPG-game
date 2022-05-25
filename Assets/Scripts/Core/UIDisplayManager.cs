using System;
using UnityEngine;
using RPG.Attributes;
using RPG.Economy;
using RPG.Traits;
using RPG.Stats;
using RPG.Inventories;
using RPG.Quests;

namespace RPG.Core
{
    /// <summary>
    /// This component provides the Static Methods to Refresh the UI display partially or all, easy calling because of static.
    /// This script only refresh according to a specific GameObject's Data. (Ex. Player GameObject's Data of Health, active Shop, QuestList)
    ///
    /// TO USE:
    /// - Setup subscriber by this example : HealthDisplay.cs subscribe to UIDisplayManager.cs THEN we subscribe our Action with Health.cs here.
    /// - Calling Public Methods : simply calling ClassName.MethodName() without the need of reference to this class.
    /// - This component Must be destroyed to clear out subscribers. Can NOT put under 'PersistentObjects' prefab.
    /// </summary>
    public class UIDisplayManager : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public static event Action OnHUDRefreshed;
        public static event Action OnInGameRefreshed;
        public static event Action OnTraitRefreshed;
        public static event Action OnShopRefreshed;
        public static event Action OnInventoryBagRefreshed;
        public static event Action OnInventoryEquipmentRefreshed;
        public static event Action OnInventoryActionRefreshed;
        public static event Action OnQuestRefreshed;

        // More check at UI SUB FOLDER
        #endregion



        #region --Fields-- (In Class)
        private BaseStats _baseStats;
        private Experience _experience;
        private Health _health;
        private Mana _mana;
        private Coin _coin;

        private TraitStore _traitStore;

        private Inventory _inventory;
        private Equipment _equipment;
        private ActionStore _actionStore;

        private QuestList _questList;
        #endregion



        #region --Fields-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _baseStats = player.GetComponentInChildren<BaseStats>();
            _experience = player.GetComponentInChildren<Experience>();
            _health = player.GetComponentInChildren<Health>();
            _mana = player.GetComponentInChildren<Mana>();
            _coin = player.GetComponentInChildren<Coin>();

            _traitStore = player.GetComponentInChildren<TraitStore>();

            _inventory = player.GetComponentInChildren<Inventory>();
            _equipment = player.GetComponentInChildren<Equipment>();
            _actionStore = player.GetComponentInChildren<ActionStore>();

            _questList = player.GetComponentInChildren<QuestList>();
        }

        private void OnEnable()
        {
            _experience.OnExperienceLoadDone += RefreshAllUI;

            _experience.OnExperienceGained += RefreshHUDUI;
            _baseStats.OnLevelUpDone += () => { RefreshHUDUI(); RefreshTraitUI(); RefreshShopUI(); }; // Need to RefreshTraitUI as well since there will be more TraitPoints given to player / RefreshShopUI incase open shop while level up
            _health.OnHealthChanged += RefreshHUDUI; // Not Subscribe with RefreshInGameUI bcuz that HealthBar is individually gets Invoke on their Health component not with Player
            _mana.OnManaPointsUpdated += RefreshHUDUI;
            _coin.OnCoinPointsUpdated += RefreshHUDUI;

            _traitStore.OnPointsChanged += () => { RefreshHUDUI(); RefreshTraitUI(); };

            // INVENTORY SYSTEM
            _inventory.OnInventoryUpdated += () => { RefreshInventoryBagUI(); };
            _equipment.OnEquipmentUpdated += () => { RefreshInventoryEquipmentUI(); RefreshHUDUI(); RefreshShopUI(); }; // RefreshShopUI incase open shop while equip new item
            _actionStore.OnStoreUpdated += () => { RefreshInventoryActionUI(); };

            // QUEST SYSTEM
            _questList.OnQuestListUpdated += RefreshQuestUI;
        }

        private void OnDisable()
        {
            // NONE of the Above Delegates are static so don't have to Unsubscribe to make it more clean

            RemoveStaticDelegatesSubscribers();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC), (Subscriber)
        public static void RefreshAllUI()
        {
            RefreshInGameUI();
            RefreshHUDUI();
            RefreshTraitUI();
            RefreshShopUI();
            RefreshInventoryBagUI();
            RefreshInventoryEquipmentUI();
            RefreshInventoryActionUI();
            RefreshQuestUI();
            print("Refreshed All UI");
        }

        public static void RefreshInGameUI() => OnInGameRefreshed?.Invoke(); // Incase need to update HealthBar for all Characters

        public static void RefreshHUDUI()
        {
            OnHUDRefreshed?.Invoke();
            //print("Refreshed HUD UI " + OnHUDRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshTraitUI()
        {
            OnTraitRefreshed?.Invoke();
            //print("Refreshed Trait UI " + OnTraitRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshShopUI()
        {
            OnShopRefreshed?.Invoke();
            //print("Refreshed Shop UI " + OnShopRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshInventoryBagUI()
        {
            OnInventoryBagRefreshed?.Invoke();
            //print("Refreshed Inventory Bag UI " + OnInventoryBagRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshInventoryEquipmentUI()
        {
            OnInventoryEquipmentRefreshed?.Invoke();
            //print("Refreshed Inventory Equipment UI " + OnInventoryEquipmentRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshInventoryActionUI()
        {
            OnInventoryActionRefreshed?.Invoke();
            //print("Refreshed Inventory Action UI " + OnInventoryActionRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshQuestUI()
        {
            OnQuestRefreshed?.Invoke();
            //print("Refreshed Quest UI " + OnQuestRefreshed?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveStaticDelegatesSubscribers()
        {
            OnHUDRefreshed = null;
            OnInGameRefreshed = null;
            OnTraitRefreshed = null;
            OnShopRefreshed = null;
            OnInventoryBagRefreshed = null;
            OnInventoryEquipmentRefreshed = null;
            OnInventoryActionRefreshed = null;
            OnQuestRefreshed = null;
        }
        #endregion
    }
}