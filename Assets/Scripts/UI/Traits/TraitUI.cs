using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Traits;
using RPG.Core;

namespace RPG.UI.Traits
{
    public class TraitUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _unallocatedPointsText;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private TraitStore _playerTraitStore;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TraitStore>();

            _confirmButton.onClick.AddListener(Commit);
        }

        private void OnEnable()
        {
            UIRefresher.OnTraitRefreshed += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnTraitRefreshed -= RefreshUI;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void Commit()
        {
            _playerTraitStore.CommitPoints();
        }

        private void RefreshUI()
        {
            _confirmButton.interactable = _playerTraitStore.CanCommit();

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            _unallocatedPointsText.text = _playerTraitStore.GetUnallocatedPoints().ToString("#,0", nfi);
        }
        #endregion
    }
}