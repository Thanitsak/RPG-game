using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Unnamed Quest", menuName = "RPG/Quest/New Quest")]
    public class Quest : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;
        [SerializeField] private int _timerInHours;
        [SerializeField] private List<string> _objectives = new List<string>();
        [SerializeField] private List<string> _rewards = new List<string>();
        #endregion



        #region --Properties-- (With Backing Up)
        public string Title { get { return _title; } }
        public string Description { get { return _description; } }
        public int TimerInHours { get { return _timerInHours; } }
        public IEnumerable<string> Objectives { get { return _objectives; } }
        public IEnumerable<string> Rewards { get { return _rewards; } }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~
        public static Quest GetByName(string name)
        {
            return Resources.Load<Quest>(name);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public bool IsObjectiveExist(string objectiveToCheck) => _objectives.Contains(objectiveToCheck);
        #endregion
    }
}