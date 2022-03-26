using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Unnamed Quest", menuName = "RPG/Quest/New Quest")]
    public class Quest : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private int _timerInHours;
        [SerializeField] private string[] _objectives;
        [SerializeField] private string[] _rewards;
        #endregion



        #region --Properties-- (With Backing Up)
        public string Title { get { return _title; } }
        public string Description { get { return _description; } }
        public int TimerInHours { get { return _timerInHours; } }
        public IEnumerable<string> Objectives { get { return _objectives; } }
        public IEnumerable<string> Rewards { get { return _rewards; } }
        #endregion
    }
}