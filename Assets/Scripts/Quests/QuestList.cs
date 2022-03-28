using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable
    {
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

        public bool AddCompletedObjective(Quest questToAddObjective, string objective)
        {
            if (!IsQuestExist(questToAddObjective)) return false;

            QuestStatus questStatus = GetQuestStatus(questToAddObjective);
            questStatus.AddCompletedObjective(objective);

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
        #endregion
    }
}