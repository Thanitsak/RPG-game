using System;
using UnityEngine;
using RPG.Attributes;
using RPG.Economy;
using RPG.Traits;
using RPG.Stats;

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

        // More check at UI SUB FOLDER
        #endregion



        #region --Fields-- (In Class)
        private BaseStats _baseStats;
        private Experience _experience;
        private Health _health;
        private Mana _mana;
        private Coin _coin;

        private TraitStore _traitStore;
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

        }

        private void OnDisable()
        {
            _experience.OnExperienceLoadDone -= RefreshAllUI;

            _experience.OnExperienceGained -= RefreshHUDUI;
            _baseStats.OnLevelUpDone -= () => { RefreshHUDUI(); RefreshTraitUI(); RefreshShopUI(); };
            _health.OnHealthChanged -= RefreshHUDUI;
            _mana.OnManaPointsUpdated -= RefreshHUDUI;
            _coin.OnCoinPointsUpdated -= RefreshHUDUI;

            _traitStore.OnPointsChanged -= () => { RefreshHUDUI(); RefreshTraitUI(); };

            RemoveAllSubscribers();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC), (Subscriber)
        public static void RefreshAllUI()
        {
            RefreshInGameUI();
            RefreshHUDUI();
            RefreshTraitUI();
            RefreshShopUI();
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
            print("Refreshed Trait UI " + OnTraitRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshShopUI()
        {
            OnShopRefreshed?.Invoke();
            print("Refreshed Shop UI " + OnShopRefreshed?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveAllSubscribers()
        {
            OnHUDRefreshed = null;
            OnInGameRefreshed = null;
            OnTraitRefreshed = null;
            OnShopRefreshed = null;
        }
        #endregion
    }
}