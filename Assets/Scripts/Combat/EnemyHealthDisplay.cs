using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _healthText;
        #endregion



        #region --Fields-- (In Class)
        private Fighter _playerFighter;
        private Health _enemyHealth;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            _enemyHealth = _playerFighter.GetTarget();

            if (_enemyHealth == null)
            {
                _healthText.text = $"N/A";
            }
            else
            {
                _healthText.text = $"{_enemyHealth.HealthPoints:N0}/{_enemyHealth.MaxHealthPoints:N0}";
                //_healthText.text = $"{_enemyHealth.GetPercentage():N0}%";
            }
        }
        #endregion
    }
}