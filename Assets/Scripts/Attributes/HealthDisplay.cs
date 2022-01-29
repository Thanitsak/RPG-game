using UnityEngine;
using TMPro;

namespace RPG.Attributes
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
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            _healthText.text = $"{_health.HealthPoints:N0}/{_health.MaxHealthPoints:N0}";
            //_healthText.text = $"{_health.GetPercentage():N0}%";
        }
        #endregion
    }
}