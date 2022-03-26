using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Quest[] _tempQuests;
        [SerializeField] private QuestItemUI _questPrefab;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            ClearQuestList();

            BuildQuestList();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BuildQuestList()
        {
            foreach (Quest eachQuest in _tempQuests)
            {
                QuestItemUI createdPrefab = Instantiate(_questPrefab, transform);
                createdPrefab.Setup(eachQuest);
            }
        }

        private void ClearQuestList()
        {
            foreach (Transform eachChild in transform)
                Destroy(eachChild.gameObject);
        }
        #endregion
    }
}