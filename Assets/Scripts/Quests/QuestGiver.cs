using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Quest _questToGive;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<QuestList>();

            questList.AddQuest(_questToGive);
        }
        #endregion
    }
}