using UnityEngine;
using TMPro;
using RPG.Stats;
using RPG.Core;

namespace RPG.UI.HUD
{
    public class LevelDisplay : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _levelText;
        #endregion



        #region --Fields-- (In Class)
        private BaseStats _baseStats;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponentInChildren<BaseStats>();
        }

        private void OnEnable()
        {
            UIRefresher.OnHUDRefreshed += UpdateLevelDisplay;
        }

        private void Start()
        {
            UpdateLevelDisplay();
        }

        private void OnDisable()
        {
            UIRefresher.OnHUDRefreshed -= UpdateLevelDisplay;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateLevelDisplay()
        {
            _levelText.text = $"{_baseStats.GetLevel()}";
        }
        #endregion
    }
}