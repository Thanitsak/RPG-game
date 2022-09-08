using System.Globalization;
using UnityEngine;
using TMPro;
using RPG.Stats;
using RPG.Core;

namespace RPG.UI.HUD
{
    public class ExperienceDisplay : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _experienceText;
        #endregion



        #region --Fields-- (In Class)
        private Experience _experience;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponentInChildren<Experience>();
        }

        private void OnEnable()
        {
            UIRefresher.OnHUDRefreshed += UpdateExperienceDisplay;
        }

        private void Start()
        {
            UpdateExperienceDisplay();
        }

        private void OnDisable()
        {
            UIRefresher.OnHUDRefreshed -= UpdateExperienceDisplay;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateExperienceDisplay()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _experienceText.text = _experience.ExperiencePoints.ToString("#,0", nfi);
        }
        #endregion
    }
}