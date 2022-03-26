using UnityEngine;
using TMPro;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestTooltip : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Texts")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private TMP_Text _descriptionText;

        [Header("Spawner Stuff")]
        [SerializeField] private TMP_Text _objectiveHeaderTextPrefab;
        [SerializeField] private TMP_Text _rewardHeaderTextPrefab;
        [SerializeField] private TMP_Text _objectivePrefab;
        [SerializeField] private Transform _objectiveTransform;
        [SerializeField] private TMP_Text _rewardPrefab;
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
                TMP_Text createdPrefab = Instantiate(_objectivePrefab, _objectiveTransform);
                createdPrefab.text = eachText;
            }
        }

        private void BuildRewardList(Quest quest)
        {
            Instantiate(_rewardHeaderTextPrefab, _rewardTransform);

            foreach (string eachText in quest.Rewards)
            {
                TMP_Text createdPrefab = Instantiate(_rewardPrefab, _rewardTransform);
                createdPrefab.text = eachText;
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