using UnityEngine;
using RPG.Quests;
using RPG.Core;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private QuestItemUI _questPrefab;
        #endregion



        #region --Fields-- (In Class)
        private QuestList _questList;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _questList = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<QuestList>();

            UIDisplayManager.OnQuestRefreshed += UpdateQuestListUI; // Can't do with OnEnable() cuz this will keep adding more and more And Since we can't use OnDisable() to unsubscribe Since this one will be closed by default and with button
        }

        private void Start()
        {
            UpdateQuestListUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BuildQuestList()
        {
            foreach (QuestStatus eachQuestStatus in _questList.QuestStatuses)
            {
                QuestItemUI createdPrefab = Instantiate(_questPrefab, transform);
                createdPrefab.Setup(eachQuestStatus);
            }
        }

        private void ClearQuestList()
        {
            foreach (Transform eachChild in transform)
                Destroy(eachChild.gameObject);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateQuestListUI()
        {
            ClearQuestList();

            BuildQuestList();
        }
        #endregion
    }
}