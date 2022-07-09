using System.Globalization;
using UnityEngine;
using TMPro;
using RPG.Attributes;
using RPG.Core;

namespace RPG.UI.HUD
{
    public class HealthDisplay : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _healthText;
        #endregion



        #region --Fields-- (In Class)
        private Health _health;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponentInChildren<Health>();
        }

        private void OnEnable()
        {
            UIDisplayManager.OnHUDRefreshed += UpdateHealthDisplay;
        }

        private void Start()
        {
            UpdateHealthDisplay();
        }

        private void OnDisable()
        {
            UIDisplayManager.OnHUDRefreshed -= UpdateHealthDisplay;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateHealthDisplay()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            
            _healthText.text = $"{_health.HealthPoints.value.ToString("#,0", nfi):N0}/{_health.MaxHealthPoints.ToString("#,0", nfi):N0}";
            //_healthText.text = $"{_health.GetPercentage():N0}%";
        }
        #endregion
    }
}