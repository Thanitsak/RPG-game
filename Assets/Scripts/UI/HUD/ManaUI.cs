using System.Globalization;
using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.UI.HUD
{
    public class ManaUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _manaPointsText;
        #endregion



        #region --Fields-- (In Class)
        private Mana _playerMana;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerMana = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Mana>();
        }

        private void OnEnable()
        {
            _playerMana.OnManaPointsUpdated += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            _playerMana.OnManaPointsUpdated -= RefreshUI;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _manaPointsText.text = _playerMana.ManaPoints.value.ToString("#,0", nfi);
        }
        #endregion
    }
}