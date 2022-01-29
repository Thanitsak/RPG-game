using UnityEngine;
using TMPro;

namespace RPG.Stats
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
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            _levelText.text = $"{_baseStats.GetLevel()}";
        }
        #endregion
    }
}