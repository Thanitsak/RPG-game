using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Traits;
using RPG.Core;

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
        private TraitStore _playerTraitStore;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TraitStore>();

            UIRefresher.OnTraitRefreshed += RefreshUI; // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button

            _minusButton.onClick.AddListener(() => Allocate(-1));
            _addButton.onClick.AddListener(() => Allocate(1));
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void Allocate(int points)
        {
            _playerTraitStore.StagePoints(_trait, points);
        }

        private void RefreshUI()
        {
            _minusButton.interactable = _playerTraitStore.CanStagePoints(_trait, -1);
            _addButton.interactable = _playerTraitStore.CanStagePoints(_trait, +1);

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _valueText.text = _playerTraitStore.GetCombinedPoints(_trait).ToString("#,0", nfi);
        }
        #endregion
    }
}