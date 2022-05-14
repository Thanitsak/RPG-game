using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Traits;

namespace RPG.UI.Traits
{
    public class TraitRowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Trait _trait;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _addButton;
        #endregion



        #region --Fields-- (In Class)
        //TEMP
        private int _value = 0;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _minusButton.onClick.AddListener(() => Allocate(-1));
            _addButton.onClick.AddListener(() => Allocate(1));
        }

        private void Update()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void Allocate(int points)
        {
            _value += points;

            _value = Mathf.Clamp(_value, 0, _value);
        }

        private void RefreshUI()
        {
            _minusButton.interactable = _value != 0;

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _valueText.text = _value.ToString("#,0", nfi);
        }
        #endregion
    }
}