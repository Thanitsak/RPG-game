using UnityEngine;
using TMPro;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _timer;
        #endregion



        #region --Fields-- (In Class)
        private Quest _quest;
        #endregion



        #region --Properties-- (With Backing Up)
        public Quest Quest { get { return _quest; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(Quest quest)
        {
            _quest = quest;

            _title.text = quest.Title;
            _timer.text = $"{quest.TimerInHours / 24} Days"; // Use Method to Convert the time here.
        }
        #endregion
    }
}