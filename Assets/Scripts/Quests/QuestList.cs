using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Saving;
using RPG.Utils.Core;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
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



        #region --Methods-- (Built In)
        private void Update()
        {
            AutoAddCompletedConditionalObjective();
        }
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

        public bool AddCompletedObjective(Quest quest, string objectiveID)
        {
            if (!IsQuestExist(quest)) return false;
            
            QuestStatus questStatus = GetQuestStatus(quest);
            if (questStatus == null) return false;
            
            if (questStatus.IsObjectiveCompleted(objectiveID)) return false;
            
            Quest.Objective objective = quest.GetObjective(objectiveID);
            if (objective == null) return false;
            if (objective.completionCondition.HasCondition() && !IsObjectiveConditionSatisfy(objective)) return false;
            
            questStatus.AddCompletedObjective(objectiveID);
            
            if (questStatus.IsQuestCompleted())
                RewardGiver.GiveReward(quest.Rewards);

            OnQuestListUpdated?.Invoke();

            return true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AutoAddCompletedConditionalObjective()
        {
            foreach (QuestStatus questStatus in _questStatuses)
            {
                if (questStatus.IsQuestCompleted()) continue;

                foreach (Quest.Objective objective in questStatus.Quest.Objectives)
                {
                    if (!objective.completionCondition.HasCondition()) continue; // filter out the one WITHOUT condition
                    if (questStatus.IsObjectiveCompleted(objective.referenceID)) continue; // filter out COMPLETED one

                    if (IsObjectiveConditionSatisfy(objective))
                    {
                        AddCompletedObjective(questStatus.Quest, objective.referenceID);
                    }
                }
            }
        }

        private bool IsObjectiveConditionSatisfy(Quest.Objective objective)
        {
            return objective.completionCondition.Check(transform.root.GetComponentsInChildren<IPredicateEvaluator>());
        }

        private bool IsObjectiveConditionSatisfy(Quest quest, string objectiveID)
        {
            Quest.Objective objective = quest.GetObjective(objectiveID);
            if (objective == null) return false;

            return IsObjectiveConditionSatisfy(objective);
        }

        private bool IsQuestExist(Quest questToCheck)
        {
            return GetQuestStatus(questToCheck) != null;
        }

        private QuestStatus GetQuestStatus(Quest questToGet)
        {
            foreach (QuestStatus eachQuestStatus in QuestStatuses)
            {
                if (questToGet == null) Debug.LogError($"GetQuestStatus() will always return null, because questToGet is null.");

                if (eachQuestStatus.Quest == questToGet)
                    return eachQuestStatus;
            }

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

        bool? IPredicateEvaluator.Evaluate(PredicateName methodName, string[] parameters)
        {
            QuestStatus questStatus = null;
            switch (methodName)
            {
                case PredicateName.HasQuest:
                    return IsQuestExist(Quest.GetByName(parameters[0]));

                case PredicateName.HasCompletedQuest:
                    questStatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (questStatus == null) return false;

                    return questStatus.IsQuestCompleted();

                case PredicateName.HasCompletedObjective:
                    questStatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (questStatus == null) return false;

                    return questStatus.IsObjectiveCompleted(parameters[1]);
            }

            return null;
        }
        #endregion
    }
}