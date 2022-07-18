using System.Collections.Generic;
using System.Linq;

namespace RPG.Quests
{
    public class QuestStatus
    {
        #region --Fields-- (In Class)
        private Quest _quest;
        private List<string> _completedObjectives = new List<string>();
        #endregion



        #region --Properties-- (With Backing Fields)
        public Quest Quest { get { return _quest; } }
        public int CompletedCount { get { return _completedObjectives.Count; } }
        #endregion



        #region --Constructors-- (PUBLIC)
        public QuestStatus(Quest quest)
        {
            _quest = quest;

            if (_quest == null) UnityEngine.Debug.LogError("QuestStatus is created with null Quest.");
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord questStatusRecord = objectState as QuestStatusRecord;
            if (questStatusRecord == null) return;

            _quest = Quest.GetByName(questStatusRecord.questName);
            _completedObjectives = questStatusRecord.completedObjectives;

            if (_quest == null) UnityEngine.Debug.LogError("QuestStatus is created with null Quest.");
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// The Added Objective String MUST be completed! This method WILL NOT do a Check for Conditional Ones. (used by QuestList.cs)
        /// </summary>
        public bool AddCompletedObjective(string objectiveToAdd)
        {
            if (IsObjectiveCompleted(objectiveToAdd)) return false;
            if (!Quest.IsObjectiveExist(objectiveToAdd))
            {
                UnityEngine.Debug.LogError($"There is no '{objectiveToAdd}' Objective in the '{Quest.name}' Quest's Objectives List");
                return false;
            }

            _completedObjectives.Add(objectiveToAdd);

            return true;
        }

        public bool IsObjectiveCompleted(string compareObjective) => _completedObjectives.Contains(compareObjective);

        public bool IsQuestCompleted()
        {
            if (Quest == null)
            {
                UnityEngine.Debug.LogError($"IsQuestCompleted() will always return false, because Quest is null.");
                return false;
            }

            return Quest.Objectives.Count() == CompletedCount;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Saving~ //for QuestList to call
        public object Capture()
        {
            QuestStatusRecord newRecord = new QuestStatusRecord();

            newRecord.questName = _quest.name;
            newRecord.completedObjectives = _completedObjectives;

            return newRecord;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives = new List<string>();
        }
        #endregion
    }
}