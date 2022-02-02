using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.UI.InGame
{
    public class HealthBar : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Slider _healthBarSlider;
        [SerializeField] private Canvas _healthBarCanvas;
        #endregion



        #region --Fields-- (In Class)
        private Health _health;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _health = GetComponentInParent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthChanged += UpdateHealthBar;
        }

        private void Start()
        {
            UpdateHealthBar();
        }

        private void OnDisable()
        {
            _health.OnHealthChanged -= UpdateHealthBar;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateHealthBar() // To use with OnHealthLoaded Action
        {
            _healthBarSlider.value = _health.GetPercentageDecimal();

            // Close when Health is FULL or EMPTY
            if (Mathf.Approximately(_health.GetPercentageDecimal(), 0f) || Mathf.Approximately(_health.GetPercentageDecimal(), 1f))
            {
                _healthBarCanvas.enabled = false;
            }
            else
            {
                _healthBarCanvas.enabled = true;
            }
        }
        #endregion
    }
}