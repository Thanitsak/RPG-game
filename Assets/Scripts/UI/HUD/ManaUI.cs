using System.Globalization;
using UnityEngine;
using TMPro;
using RPG.Attributes;
using RPG.Core;

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
            UIRefresher.OnHUDRefreshed += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnHUDRefreshed -= RefreshUI;
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