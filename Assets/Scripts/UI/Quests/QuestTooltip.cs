using UnityEngine;
using TMPro;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestTooltip : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Main Header Texts")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private TMP_Text _descriptionText;

        [Header("Sub Header Texts")]
        [SerializeField] private TMP_Text _objectiveHeaderTextPrefab;
        [SerializeField] private TMP_Text _rewardHeaderTextPrefab;

        [Header("Spawner Stuff")]
        [SerializeField] private GameObject _objectivePrefab;
        [SerializeField] private Transform _objectiveTransform;
        [SerializeField] private GameObject _rewardPrefab;
        [SerializeField] private Transform _rewardTransform;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(Quest quest)
        {
            _headerText.text = quest.Title;
            _descriptionText.text = quest.Description;

            ClearObjectiveList();
            ClearRewardList();

            BuildObjectiveList(quest);
            BuildRewardList(quest);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BuildObjectiveList(Quest quest)
        {
            Instantiate(_objectiveHeaderTextPrefab, _objectiveTransform);

            foreach (string eachText in quest.Objectives)
            {
                TMP_Text createdPrefabText = Instantiate(_objectivePrefab, _objectiveTransform).GetComponentInChildren<TMP_Text>();
                createdPrefabText.text = eachText;
            }
        }

        private void BuildRewardList(Quest quest)
        {
            Instantiate(_rewardHeaderTextPrefab, _rewardTransform);

            foreach (string eachText in quest.Rewards)
            {
                TMP_Text createdPrefabText = Instantiate(_rewardPrefab, _rewardTransform).GetComponentInChildren<TMP_Text>();
                createdPrefabText.text = eachText;
            }
        }

        private void ClearObjectiveList()
        {
            foreach (Transform eachChild in _objectiveTransform)
                Destroy(eachChild.gameObject);
        }

        private void ClearRewardList()
        {
            foreach (Transform eachChild in _rewardTransform)
                Destroy(eachChild.gameObject);
        }
        #endregion
    }
}