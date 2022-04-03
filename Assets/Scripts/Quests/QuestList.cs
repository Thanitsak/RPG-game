using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Saving;
using RPG.Inventories;
using RPG.Core;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        #region --Fields-- (Inspector)
        //[Header("Method Name Filter")]
        //[Tooltip("This String will be compared with String put in Dialogue Node also with parameter value AND if it matches then that Dialogue Node can be included")]
        //[SerializeField] private string _isQuestExistFilter = "IsQuestExist";
        //[SerializeField] private string _isQuestCompletedFilter = "IsQuestCompleted";
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnQuestListUpdated;
        #endregion



        #region --Fields-- (In Class)
        private List<QuestStatus> _questStatuses = new List<QuestStatus>();
        #endregion



        #region --Properties-- (With Backing Fields)
        public IEnumerable<QuestStatus> QuestStatuses { get { return _questStatuses; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public bool AddQuest(Quest questToGive)
        {
            if (IsQuestExist(questToGive)) return false;

            QuestStatus newQuestStatus = new QuestStatus(questToGive);
            _questStatuses.Add(newQuestStatus);

            OnQuestListUpdated?.Invoke();

            return true;
        }

        public bool AddCompletedObjective(Quest quest, string objective)
        {
            if (!IsQuestExist(quest)) return false;
            
            QuestStatus questStatus = GetQuestStatus(quest);
            if (questStatus == null) return false;

            if (questStatus.IsObjectiveCompleted(objective)) return false;

            questStatus.AddCompletedObjective(objective);

            if (questStatus.IsQuestCompleted())
            {
                GiveQuestReward(quest);
            }

            OnQuestListUpdated?.Invoke();

            return true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsQuestExist(Quest questToCheck)
        {
            return GetQuestStatus(questToCheck) != null;
        }

        private QuestStatus GetQuestStatus(Quest questToGet)
        {
            foreach (QuestStatus eachQuestStatus in QuestStatuses)
                if (eachQuestStatus.Quest == questToGet)
                    return eachQuestStatus;

            return null;
        }

        private void GiveQuestReward(Quest quest)
        {
            // For Each of Reward, Gradually Add one Reward to empty slot, OR IF FULL drop that one down. (Stackable or Non-Stackable can both be done like this)
            foreach (Quest.Reward eachReward in quest.Rewards)
            {
                for (int i = 0; i < eachReward.number; i++)
                {
                    bool success = GetComponentInChildren<Inventory>().AddToFirstEmptySlot(eachReward.rewardItem, 1);
                    if (!success)
                        GetComponentInChildren<ItemDropper>().DropItem(eachReward.rewardItem, 1);
                }
            }
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            List<object> savedState = new List<object>();
            foreach (QuestStatus eachQuestStatus in QuestStatuses)
            {
                savedState.Add(eachQuestStatus.Capture());
            }

            return savedState;
        }

        void ISaveable.RestoreState(object state)
        {
            List<object> loadedState = (List<object>)state;

            _questStatuses.Clear();
            foreach (object eachState in loadedState)
            {
                _questStatuses.Add(new QuestStatus(eachState));
            }

            OnQuestListUpdated?.Invoke();
        }

        bool? IPredicateEvaluator.Evaluate(string methodName, string[] parameters)
        {
            switch (methodName)
            {
                case "HasQuest":
                    return IsQuestExist(Quest.GetByName(parameters[0]));

                case "HasCompletedQuest":
                    QuestStatus questStatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (questStatus == null) return false;

                    return questStatus.IsQuestCompleted();
            }

            return null;
        }
        #endregion
    }
}