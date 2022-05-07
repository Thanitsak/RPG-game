using System;
using UnityEngine;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public event Action OnManaPointsUpdated;
        #endregion



        #region --Fields-- (In Class)
        private BaseStats _baseStats;
        #endregion



        #region --Properties-- (Auto)
        public AutoInit<float> ManaPoints { get; private set; }
        #endregion



        #region --Properties-- (With Backing Fields)
        public float MaxManaPoints { get { return _baseStats.GetMana(); } }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _baseStats = transform.root.GetComponentInChildren<BaseStats>();

            ManaPoints = new AutoInit<float>(GetInitialMana);
        }

        private void Start()
        {
            ManaPoints.ForceInit();
        }

        private void Update()
        {
            RegenerateMana();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public bool UseMana(float amount)
        {
            if (amount > ManaPoints.value) return false;

            ManaPoints.value -= amount;
            OnManaPointsUpdated?.Invoke();

            return true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RegenerateMana()
        {
            if (ManaPoints.value >= MaxManaPoints) return;

            ManaPoints.value += Time.deltaTime * _baseStats.GetManaRegenRate();
            ManaPoints.value = Mathf.Clamp(ManaPoints.value, 0f, MaxManaPoints);

            OnManaPointsUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private float GetInitialMana() => MaxManaPoints;
        #endregion
    }
}