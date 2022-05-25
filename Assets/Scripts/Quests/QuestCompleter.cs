using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompleter : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Quest _questWithObjective;
        [SerializeField] private string _objectiveToComplete;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void CompleteQuestObjective()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<QuestList>();

            questList.AddCompletedObjective(_questWithObjective, _objectiveToComplete);
        }
        #endregion
    }
}