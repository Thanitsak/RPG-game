using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using RPG.Utils.Core;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Untitled Quest", menuName = "RPG/Quest/New Quest", order = 3)]
    public class Quest : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;
        [SerializeField] private int _timerInHours;
        [SerializeField] private List<Objective> _objectives = new List<Objective>();
        [SerializeField] private List<Reward> _rewards = new List<Reward>();
        #endregion



        #region --Fields-- (In Class)
        private static Dictionary<string, Quest> _questLookupCache;
        #endregion



        #region --Properties-- (With Backing Up)
        public string Title { get { return _title; } }
        public string Description { get { return _description; } }
        public int TimerInHours { get { return _timerInHours; } }
        public IEnumerable<Objective> Objectives { get { return _objectives; } }
        public IEnumerable<Reward> Rewards { get { return _rewards; } }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~
        public static Quest GetByName(string questName)
        {
            if (_questLookupCache == null)
            {
                _questLookupCache = new Dictionary<string, Quest>();
                Quest[] questList = Resources.LoadAll<Quest>("");
                foreach (Quest quest in questList)
                {
                    if (_questLookupCache.ContainsKey(quest.name))
                    {
                        Debug.LogError($"Quest are duplicate! For: {quest.name} and {questName}.");
                        continue;
                    }

                    _questLookupCache.Add(quest.name, quest);
                }
                if (questList.Length == 0) Debug.LogError($"Resources can't find any Quest ScriptableObject, so return as null.");
            }
            if (questName != null && !_questLookupCache.ContainsKey(questName)) Debug.LogError($"Resources can't find a Quest ScriptableObject: {questName}, so will return as null.");
            if (questName == null || !_questLookupCache.ContainsKey(questName)) return null;

            return _questLookupCache[questName];
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public Objective GetObjective(string objectiveID)
        {
            if (!IsObjectiveExist(objectiveID)) return null;

            foreach (Objective eachObjective in Objectives)
                if (eachObjective.referenceID == objectiveID)
                    return eachObjective;

            return null;
        }

        public bool IsObjectiveExist(string objectiveID)
        {
            foreach (Objective eachObjective in Objectives)
                if (eachObjective.referenceID == objectiveID)
                    return true;

            return false;
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        [System.Serializable]
        public class Objective
        {
            [Tooltip("This will be used at : " +
                "\n- 'QuestCompleter' components" +
                "\n- at any 'condition' that checks for HasCompletedObjective")]
            public string referenceID;
            public string description;
            public Condition completionCondition;
        }

        [System.Serializable]
        public class Reward
        {
            public InventoryItem rewardItem;
            [Min(1)]
            public int number;
            public string description;
        }
        #endregion
    }
}